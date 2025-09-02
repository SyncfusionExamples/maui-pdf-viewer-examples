﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InsertToolbarItems
{
    /// <summary>
    /// Provides the functionality to open and save PDF files. 
    /// </summary>
    public partial class FileService
    {
        /// <summary>
        /// Shows a file picker and lets the user choose the required PDF file. 
        /// </summary>
        /// <returns>The data of the PDF file that is chosen by the user.</returns>
        public async static Task<PdfFileData?> OpenFileRequest(string fileExtension)
        {
            // Define platform-specific file types for the file picker.
            FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.iOS, new[] { $"com.adobe.{fileExtension}" } },
                        { DevicePlatform.Android, new[] { $"application/{fileExtension}" } },
                        { DevicePlatform.WinUI, new[] { fileExtension } },
                        { DevicePlatform.MacCatalyst, new[] { fileExtension } },
                    });

            // Configure the file picker options.
            PickOptions options = new()
            {
                PickerTitle = "Please select a PDF file",
            };

            // Set file type for the pick option to pick the file.
#if !ANDROID
            options.FileTypes = pdfFileType;
#else
            if (fileExtension == "pdf")
                options.FileTypes = pdfFileType;
#endif

            // Call the method to pick the file and return the result.
            return await PickFile(options, fileExtension);
        }
        static async Task<PdfFileData?> PickFile(PickOptions options, string fileExtension)
        {
            try
            {
                // Launch the file picker and wait for user selection.
                var result = await FilePicker.Default.PickAsync(options);

                // Check if a file was selected.
                if (result != null)
                {
                    // Ensure the file has a name.
                    if (result.FileName != null)
                    {
                        // Validate the file extension (case-insensitive).
                        if (result.FileName.EndsWith($".{fileExtension}", StringComparison.OrdinalIgnoreCase))
                        {
                            // Return a new PdfFileData object with the file name and stream.
                            return new PdfFileData(result.FileName, await result.OpenReadAsync());
                        }
                        else
                            Application.Current?.Windows[0].Page?.DisplayAlert("Error", $"Pick a file of type {fileExtension}", "OK");
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                string message;
                if (ex != null && string.IsNullOrEmpty(ex.Message) == false)
                    message = ex.Message;
                else
                    message = "File open failed.";
                Application.Current?.Windows[0].Page?.DisplayAlert("Error", message, "OK");
            }
            return null;
        }
        /// <summary>
        /// Saves the PDF stream with given file name using platform specific file saving APIs. 
        /// </summary>
        /// <param name="fileName">The file name with which the PDF needs to be saved.</param>
        /// <param name="fileStream">The stream of the PDF to be saved.</param>
        /// <returns></returns>
        public async static Task<string?> SaveAsAsync(string fileName, Stream fileStream)
        {
            return await PlatformSaveAsAsync(fileName, fileStream);
        }

        /// <summary>
        /// Writes the given stream to the specified file path.
        /// </summary>
        /// <param name="stream">The stream of the PDF file to be written.</param>
        /// <param name="filePath">The file path to which the PDF file needs to be written.</param>
        /// <returns></returns>
        static async Task WriteStream(Stream stream, string? filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                await using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                fileStream.SetLength(0);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                await stream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }
    }
}
