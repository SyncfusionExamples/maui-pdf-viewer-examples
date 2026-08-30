using System.IO;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace MauiApp5.PdfViewer;

/// <summary>
/// Provides offline storage and management of PDF documents.
/// Supports downloading PDFs for offline use, tracking downloaded files,
/// and managing the local PDF cache.
/// </summary>
public class PdfDocumentManager
{
    private const string PdfCacheFolder = "PdfCache";
    private const string MetadataFile = "pdf_metadata.json";

    /// <summary>
    /// Gets the path to the local PDF cache directory.
    /// </summary>
    public string CacheDirectory
    {
        get
        {
            var dir = Path.Combine(FileSystem.AppDataDirectory, PdfCacheFolder);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
    }

    /// <summary>
    /// Saves a PDF stream to the local cache for offline use.
    /// </summary>
    public async Task<string> SaveForOfflineAsync(Stream pdfStream, string fileName)
    {
        if (pdfStream == null)
            throw new ArgumentNullException(nameof(pdfStream));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

        // Sanitize the filename
        fileName = SanitizeFileName(fileName);

        var filePath = Path.Combine(CacheDirectory, fileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await pdfStream.CopyToAsync(fileStream);

        // Update metadata
        await UpdateMetadataAsync(fileName, filePath);

        return filePath;
    }

    /// <summary>
    /// Saves a PDF from bytes to the local cache for offline use.
    /// </summary>
    public async Task<string> SaveForOfflineAsync(byte[] pdfBytes, string fileName)
    {
        if (pdfBytes == null)
            throw new ArgumentNullException(nameof(pdfBytes));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

        var filePath = Path.Combine(CacheDirectory, SanitizeFileName(fileName));

        await File.WriteAllBytesAsync(filePath, pdfBytes);

        // Update metadata
        await UpdateMetadataAsync(fileName, filePath);

        return filePath;
    }

    /// <summary>
    /// Checks whether a PDF with the given file name is available in the offline cache.
    /// </summary>
    public bool IsAvailableOffline(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var filePath = Path.Combine(CacheDirectory, SanitizeFileName(fileName));
        return File.Exists(filePath);
    }

    /// <summary>
    /// Gets the full path to an offline PDF if it exists in the cache.
    /// Returns null if the PDF is not cached.
    /// </summary>
    public string GetOfflinePath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;

        var filePath = Path.Combine(CacheDirectory, SanitizeFileName(fileName));
        return File.Exists(filePath) ? filePath : null;
    }

    /// <summary>
    /// Deletes a PDF from the offline cache.
    /// </summary>
    public void DeleteOffline(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        var filePath = Path.Combine(CacheDirectory, SanitizeFileName(fileName));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Remove from metadata
        Task.Run(async () => await RemoveFromMetadataAsync(fileName)).Wait();
    }

    /// <summary>
    /// Gets all PDF files currently cached for offline use.
    /// </summary>
    public List<PdfMetadata> GetCachedPdfs()
    {
        try
        {
            var metadataPath = Path.Combine(CacheDirectory, MetadataFile);
            if (!File.Exists(metadataPath))
                return new List<PdfMetadata>();

            var json = File.ReadAllText(metadataPath);
            return JsonSerializer.Deserialize<List<PdfMetadata>>(json) ?? new List<PdfMetadata>();
        }
        catch
        {
            return new List<PdfMetadata>();
        }
    }

    /// <summary>
    /// Clears the entire PDF cache.
    /// </summary>
    public void ClearCache()
    {
        if (Directory.Exists(CacheDirectory))
        {
            Directory.Delete(CacheDirectory, recursive: true);
            Directory.CreateDirectory(CacheDirectory);
        }
    }

    /// <summary>
    /// Gets the total size of the PDF cache in bytes.
    /// </summary>
    public long GetCacheSize()
    {
        if (!Directory.Exists(CacheDirectory))
            return 0;

        return Directory.GetFiles(CacheDirectory, "*.pdf")
            .Sum(file => new FileInfo(file).Length);
    }

    private async Task UpdateMetadataAsync(string fileName, string filePath)
    {
        try
        {
            var metadataPath = Path.Combine(CacheDirectory, MetadataFile);
            var metadata = GetCachedPdfs();

            var existing = metadata.FirstOrDefault(m => m.FileName == fileName);
            if (existing != null)
            {
                existing.CachedAt = DateTime.UtcNow;
                existing.FilePath = filePath;
            }
            else
            {
                metadata.Add(new PdfMetadata
                {
                    FileName = fileName,
                    FilePath = filePath,
                    CachedAt = DateTime.UtcNow
                });
            }

            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metadataPath, json);
        }
        catch
        {
            // Metadata updates are best-effort; don't fail the save operation
        }
    }

    private async Task RemoveFromMetadataAsync(string fileName)
    {
        try
        {
            var metadataPath = Path.Combine(CacheDirectory, MetadataFile);
            var metadata = GetCachedPdfs();
            metadata.RemoveAll(m => m.FileName == fileName);
            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metadataPath, json);
        }
        catch
        {
            // Best-effort cleanup
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return $"document_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

        // Remove invalid path characters
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));

        // Ensure .pdf extension
        if (!sanitized.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            sanitized += ".pdf";

        return sanitized;
    }
}

/// <summary>
/// Metadata entry for a cached PDF document.
/// </summary>
public class PdfMetadata
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime CachedAt { get; set; }
    public long FileSizeBytes { get; set; }
}
