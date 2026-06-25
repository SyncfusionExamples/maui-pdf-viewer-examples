using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Storage;

namespace MauiApp5.PdfViewer;

/// <summary>
/// File service for handling PDF file operations (open and save).
/// Based on Syncfusion's Getting Started pattern.
/// </summary>
public class PdfFileData
{
    public required string FileName { get; set; }
    public required Stream Stream { get; set; }
}

public partial class FileService
{
    /// <summary>
    /// Opens a file picker and returns the selected PDF file data.
    /// </summary>
    public static async Task<PdfFileData?> OpenFile(string fileType)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/pdf" } },
                    { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
                    { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                    { DevicePlatform.WinUI, new[] { ".pdf" } },
                }),
                PickerTitle = "Select a PDF file"
            });

            if (result == null)
                return null;

            var stream = await result.OpenReadAsync();
            return new PdfFileData
            {
                FileName = result.FileName,
                Stream = stream
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FileService] OpenFile failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Saves a stream to a file using platform-specific file picker.
    /// Returns the full file path where the file was saved.
    /// </summary>
    public static async Task<string?> SaveAsAsync(string fileName, Stream stream)
    {
        try
        {
            return await PlatformSaveAsAsync(fileName, stream);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FileService] SaveAsAsync failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Platform-specific implementation of SaveAsAsync.
    /// Each platform provides its own implementation via partial classes.
    /// </summary>
    private static partial Task<string> PlatformSaveAsAsync(string fileName, Stream stream);

    /// <summary>
    /// Writes stream data to a file path.
    /// Used by platform-specific implementations.
    /// </summary>
    protected static async Task WriteStream(Stream stream, string filePath)
    {
        using (var fileStream = File.Create(filePath))
        {
            await stream.CopyToAsync(fileStream);
        }
    }

    /// <summary>
    /// Gets all cached PDF files from the application's data directory.
    /// </summary>
    public static List<string> GetCachedPdfFiles()
    {
        var pdfFiles = new List<string>();
        var appDataPath = FileSystem.AppDataDirectory;

        if (Directory.Exists(appDataPath))
        {
            var files = Directory.GetFiles(appDataPath, "*.pdf");
            pdfFiles.AddRange(files);
        }

        return pdfFiles;
    }
}
