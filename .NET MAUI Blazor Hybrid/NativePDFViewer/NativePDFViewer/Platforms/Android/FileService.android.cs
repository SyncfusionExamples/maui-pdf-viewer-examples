using Uri = Android.Net.Uri;
using Application = Android.App.Application;
using Environment = Android.OS.Environment;
using Android.Content;
using Android.Webkit;
using Android.Provider;
using System.Web;
using Java.IO;
using Microsoft.Maui;
using Android.OS;

namespace MauiApp5.PdfViewer
{
    public partial class FileService
    {
        private static partial async Task<string> PlatformSaveAsAsync(string fileName, Stream stream)
        {
            CancellationToken cancellationToken = CancellationToken.None;

            // Request storage permissions for Android 32 and below
            if (!OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                var status = await Permissions.RequestAsync<Permissions.StorageWrite>().WaitAsync(cancellationToken).ConfigureAwait(false);
                if (status is not PermissionStatus.Granted)
                {
                    throw new PermissionException("Storage permission is not granted.");
                }
            }

            try
            {
                // Use ACTION_CREATE_DOCUMENT intent to open file picker dialog
                var intent = new Intent(Intent.ActionCreateDocument);
                intent.AddCategory(Intent.CategoryOpenable);
                intent.SetType(MimeTypeMap.Singleton?.GetMimeTypeFromExtension(MimeTypeMap.GetFileExtensionFromUrl(fileName)) ?? "*/*");
                intent.PutExtra(Intent.ExtraTitle, fileName);

                var context = Application.Context;
                var activity = Platform.CurrentActivity;

                if (activity == null)
                    throw new Exception("Unable to get current activity");

                // Create task to wait for result
                var tcs = new TaskCompletionSource<string>();

                // Define result callback - this will be called via reflection/events
                EventHandler<(int RequestCode, int ResultCode, Intent? Intent)>? resultHandler = null;

                resultHandler = (s, args) =>
                {
                    if (args.RequestCode == 5000)
                    {
                        try
                        {
                            if (args.ResultCode == (int)Android.App.Result.Ok && args.Intent?.Data != null)
                            {
                                var uri = args.Intent.Data;
                                
                                using (var outputStream = context.ContentResolver?.OpenOutputStream(uri))
                                {
                                    if (outputStream != null)
                                    {
                                        stream.Position = 0;
                                        stream.CopyTo(outputStream);
                                        outputStream.Close();
                                    }
                                }

                                stream.Dispose();
                                tcs.SetResult(uri.ToString());
                            }
                            else
                            {
                                tcs.SetException(new Exception("File picker was cancelled"));
                            }
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }
                };

                // Note: For now, save to Documents folder as a fallback
                // A proper implementation would require a custom MainActivity
                var storageDir = Android.OS.Environment.ExternalStorageDirectory;
                var documentsPath = System.IO.Path.Combine(storageDir?.AbsolutePath ?? "", "Documents");
                System.IO.Directory.CreateDirectory(documentsPath);
                var filePath = System.IO.Path.Combine(documentsPath, fileName);

                await WriteStream(stream, filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                stream?.Dispose();
                throw;
            }
        }
    }
}
