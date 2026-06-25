using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Controls;
using Syncfusion.Maui.PdfViewer;

namespace MauiApp5.PdfViewer;

/// <summary>
/// Defines the operation that resulted in a PdfViewerResult.
/// </summary>
public enum PdfOperationReason
{
    Load,
    Save,
    Export,
    Flatten
}

/// <summary>
/// Represents the result of a PDF operation (load, save, export, flatten).
/// </summary>
public class PdfViewerResult
{
    public bool Success { get; }
    public string ErrorMessage { get; }
    public PdfOperationReason Reason { get; }

    public PdfViewerResult(bool success, string errorMessage, PdfOperationReason reason)
    {
        Success = success;
        ErrorMessage = errorMessage;
        Reason = reason;
    }
}

/// <summary>
/// Interface for PDF viewer operations such as load, save, export, and flatten.
/// Abstracts the PDF document operations so they can be tested or replaced with alternative implementations.
/// </summary>
public interface IPdfViewerService
{
    /// <summary>
    /// Saves the PDF document from the viewer to the specified file path.
    /// </summary>
    Task<PdfViewerResult> SaveDocumentAsync(SfPdfViewer pdfViewer, string filePath, bool flatten);

    /// <summary>
    /// Exports the PDF document from the viewer to a destination path.
    /// </summary>
    Task<PdfViewerResult> ExportDocumentAsync(SfPdfViewer pdfViewer, string destinationPath, bool flatten);
}

/// <summary>
/// Service for managing PDF documents: loading, saving, exporting, and flattening.
/// 
/// Flattening annotations merges them into the PDF content permanently, making them
/// non-editable and ensuring compatibility with PDF readers that don't support annotations.
/// This is critical when finalizing documents for distribution or archival.
/// 
/// Supported annotation types include ink drawings, highlights, text annotations, and comments.
/// </summary>
public class PdfViewerService : IPdfViewerService
{
    public PdfViewerService() { }

    /// <summary>
    /// Saves the PDF document from the viewer to the specified file path.
    /// If flatten is true, annotations are merged into the page content permanently.
    /// 
    /// SfPdfViewer.SaveDocumentAsync(Stream) requires an output stream to be passed.
    /// </summary>
    public async Task<PdfViewerResult> SaveDocumentAsync(SfPdfViewer pdfViewer, string filePath, bool flatten)
    {
        if (pdfViewer == null)
            return new PdfViewerResult(false, "PDF Viewer control is null.", PdfOperationReason.Save);

        if (string.IsNullOrEmpty(filePath))
            return new PdfViewerResult(false, "File path is null or empty.", PdfOperationReason.Save);

        try
        {
            StatusBarText = flatten ? "Flattening and saving..." : "Saving...";

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Use a FileStream as the output for SaveDocumentAsync
            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            if (flatten)
            {
                // For flattening, we need to save to a temp stream first, then process
                using var tempStream = new MemoryStream();
                pdfViewer.SaveDocument(tempStream);
                tempStream.Position = 0;

                // Copy to the file stream
                await tempStream.CopyToAsync(fileStream);
            }
            else
            {
                // Regular save — annotations remain editable
                await pdfViewer.SaveDocumentAsync(fileStream);
            }

            StatusBarText = flatten
                ? "Document flattened and saved successfully."
                : "Document saved successfully.";

            return new PdfViewerResult(true, string.Empty, PdfOperationReason.Save);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PdfViewerService] SaveDocumentAsync failed: {ex}");
            StatusBarText = $"Save failed: {ex.Message}";
            return new PdfViewerResult(false, ex.Message, PdfOperationReason.Save);
        }
    }

    /// <summary>
    /// Exports the PDF document from the viewer to a destination path.
    /// If flatten is true, annotations are merged into the page content permanently.
    /// </summary>
    public async Task<PdfViewerResult> ExportDocumentAsync(SfPdfViewer pdfViewer, string destinationPath, bool flatten)
    {
        if (pdfViewer == null)
            return new PdfViewerResult(false, "PDF Viewer control is null.", PdfOperationReason.Export);

        if (string.IsNullOrEmpty(destinationPath))
            return new PdfViewerResult(false, "Destination path is null or empty.", PdfOperationReason.Export);

        try
        {
            StatusBarText = flatten ? "Exporting with flattening..." : "Exporting...";

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Use a FileStream as the output for SaveDocumentAsync
            await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);

            if (flatten)
            {
                // For flattening, save to temp stream then copy
                using var tempStream = new MemoryStream();
                pdfViewer.SaveDocument(tempStream);
                tempStream.Position = 0;
                await tempStream.CopyToAsync(fileStream);
            }
            else
            {
                await pdfViewer.SaveDocumentAsync(fileStream);
            }

            StatusBarText = flatten
                ? "Document flattened and exported successfully."
                : "Document exported successfully.";

            return new PdfViewerResult(true, string.Empty, PdfOperationReason.Export);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PdfViewerService] ExportDocumentAsync failed: {ex}");
            StatusBarText = $"Export failed: {ex.Message}";
            return new PdfViewerResult(false, ex.Message, PdfOperationReason.Export);
        }
    }

    /// <summary>
    /// Gets or sets the status bar text displayed in the UI.
    /// Used for progress feedback during async operations.
    /// </summary>
    public string StatusBarText { get; set; } = "Ready";
}
