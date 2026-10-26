namespace ViewOnlyPdf
{
    public partial class FileService
    {
        internal static partial async Task<string?> PlatformSaveAsAsync(string fileName, Stream stream)
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                return null;
            }

            string downloads = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath;
            string filePath = Path.Combine(downloads, fileName);
            await WriteStream(stream, filePath);
            return filePath;
        }
    }
}
