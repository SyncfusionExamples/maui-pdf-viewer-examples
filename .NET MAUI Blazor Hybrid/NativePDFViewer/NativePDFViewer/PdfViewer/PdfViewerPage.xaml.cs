using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Controls;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Pdf.Parsing;
using DataFormat = Syncfusion.Pdf.Parsing.DataFormat;

namespace MauiApp5.PdfViewer;

/// <summary>
/// Native MAUI PDF viewer page that supports annotation, save, and flatten operations.
/// Works independently of the Blazor WebView — this is a true native MAUI page that can be
/// navigated to from Blazor via INavigation, providing the full PDF workflow:
/// 1. Open/view PDFs on mobile
/// 2. Annotate/mark up PDFs
/// 3. Save/export edited PDFs
/// 4. Flatten annotations during save/finalization
/// 5. Reopen saved PDFs
/// 6. Support offline use after PDF is available on device
/// 
/// Save operations are based on Syncfusion's Getting Started pattern.
/// </summary>
public partial class PdfViewerPage : ContentPage
{
    private readonly IPdfViewerService _pdfViewerService;
    private string? _currentFilePath;
    private string? _currentFileName;
    private bool _hasUnsavedChanges;
    private FileStream? _currentFileStream;

    public PdfViewerPage(string pdfPath)
    {
        InitializeComponent();
        _pdfViewerService = MauiProgram.ServiceProvider?.GetService(typeof(IPdfViewerService)) as IPdfViewerService
                             ?? new PdfViewerService();
        _currentFilePath = pdfPath;
        Title = Path.GetFileName(pdfPath);

        // Load the document after the page is initialized
        this.Loaded += async (s, e) => LoadDocument(pdfPath);

        // Clean up resources when page is unloaded
        this.Unloaded += (s, e) =>
        {
            _currentFileStream?.Dispose();
            _currentFileStream = null;
        };
    }

    public PdfViewerPage()
    {
        InitializeComponent();
        _pdfViewerService = MauiProgram.ServiceProvider?.GetService(typeof(IPdfViewerService)) as IPdfViewerService
                             ?? new PdfViewerService();
    }

    /// <summary>
    /// Loads a PDF document into the viewer.
    /// SfPdfViewer.LoadDocument accepts a Stream in .NET MAUI.
    /// NOTE: We keep the FileStream open (_currentFileStream) because SfPdfViewer needs it
    /// to remain accessible. The stream is disposed when the page unloads or a new document is loaded.
    /// </summary>
    public void LoadDocument(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            StatusLabel.Text = "File not found.";
            return;
        }

        _currentFilePath = filePath;
        _currentFileName = Path.GetFileName(filePath);
        Title = _currentFileName;
        StatusLabel.Text = "Loading document...";

        try
        {
            // Dispose previous stream if any
            _currentFileStream?.Dispose();
            _currentFileStream = null;

            // Open a new stream and keep it open
            _currentFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _currentFileStream.Position = 0;
            PdfViewerControl.LoadDocument(_currentFileStream);
            _hasUnsavedChanges = false;
            StatusLabel.Text = "Document loaded successfully.";
            PageInfoLabel.Text = $"Pages: {PdfViewerControl.PageCount}";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Failed to load: {ex.Message}";
            Debug.WriteLine($"[PdfViewerPage] LoadDocumentAsync failed: {ex}");
            _currentFileStream?.Dispose();
            _currentFileStream = null;
        }
    }

    /// <summary>
    /// Loads a PDF document from a stream (used by Getting Started file picker pattern).
    /// </summary>
    public void LoadDocumentFromStream(Stream stream, string fileName)
    {
        if (stream == null)
        {
            StatusLabel.Text = "Stream is null.";
            return;
        }

        _currentFileName = fileName;
        _currentFilePath = null;
        Title = fileName;
        StatusLabel.Text = "Loading document...";

        try
        {
            // Dispose previous stream if any
            _currentFileStream?.Dispose();
            
            // For stream-based documents, we need to keep track of the new stream
            stream.Position = 0;
            PdfViewerControl.LoadDocument(stream);
            _currentFileStream = stream as FileStream;
            _hasUnsavedChanges = false;
            StatusLabel.Text = "Document loaded successfully.";
            PageInfoLabel.Text = $"Pages: {PdfViewerControl.PageCount}";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Failed to load: {ex.Message}";
            Debug.WriteLine($"[PdfViewerPage] LoadDocumentFromStream failed: {ex}");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        if (_hasUnsavedChanges)
        {
            bool shouldSave = await DisplayAlertAsync(
                "Unsaved Changes",
                "You have unsaved changes. Do you want to save before leaving?",
                "Save", "Discard");

            if (shouldSave)
            {
                await SaveDocumentAsync(flatten: false);
            }
        }

        await Navigation.PopAsync();
    }


    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await SaveDocumentWithFilePickerAsync();
    }

    private async void OnFlattenSaveClicked(object sender, EventArgs e)
    {
        await SaveDocumentWithFilePickerAsync(true);
    }

    /// <summary>
    /// Saves the PDF document using the Getting Started pattern.
    /// Saves to MemoryStream first, then uses FileService.SaveAsAsync() for file picker dialog.
    /// Flattens all annotations and form fields on save.
    /// </summary>
    private async Task SaveDocumentWithFilePickerAsync(bool flatten = false)
    {
        if (PdfViewerControl == null)
        {
            await DisplayAlertAsync("Error", "PDF viewer not initialized.", "OK");
            return;
        }

        StatusLabel.Text = "Flattening annotations and saving document...";

        try
        {
            if (flatten)
            {
                // Flatten all annotations
                var annotations = PdfViewerControl.Annotations;
                foreach (var annotation in annotations)
                {
                    annotation.FlattenOnSave = true;
                }

                // Flatten all form fields
                var formFields = PdfViewerControl.FormFields;
                foreach (var formField in formFields)
                {
                    formField.FlattenOnSave = true;
                }
            }

            // Create a memory stream to hold the saved PDF
            Stream savedStream = new MemoryStream();
            
            // Save the document to the memory stream (with annotations and form fields flattened)
            await PdfViewerControl.SaveDocumentAsync(savedStream);

            // Determine the filename to suggest
            string suggestedFileName = string.IsNullOrEmpty(_currentFileName)
                ? "document.pdf"
                : _currentFileName;

            // Use FileService to show file picker and save
            string? filePath = await FileService.SaveAsAsync(suggestedFileName, savedStream);

            if (!string.IsNullOrEmpty(filePath))
            {
                _hasUnsavedChanges = false;
                _currentFileName = Path.GetFileName(filePath);
                StatusLabel.Text = $"✓ Saved: {_currentFileName}";
#if NET10_0_OR_GREATER
                await DisplayAlertAsync("Success", $"File saved to:\n{filePath}\n(Annotations & form fields flattened)", "OK");
#else
                await DisplayAlert("Success", $"File saved to:\n{filePath}\n(Annotations & form fields flattened)", "OK");
#endif
            }
            else
            {
                StatusLabel.Text = "Save cancelled or failed.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PdfViewerPage] SaveDocumentWithFilePickerAsync failed: {ex}");
            StatusLabel.Text = $"Save failed: {ex.Message}";
#if NET10_0_OR_GREATER
            await DisplayAlertAsync("Error", $"Failed to save: {ex.Message}", "OK");
#else
            await DisplayAlert("Error", $"Failed to save: {ex.Message}", "OK");
#endif
        }
    }

    /// <summary>
    /// Saves the PDF document, optionally flattening annotations.
    /// Flattening makes annotations permanent (non-editable) in the saved file.
    /// </summary>
    public async Task<PdfViewerResult> SaveDocumentAsync(bool flatten)
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            return new PdfViewerResult(false, "No document loaded.", PdfOperationReason.Save);
        }

        StatusLabel.Text = flatten ? "Flattening and saving..." : "Saving...";

        try
        {
            var result = await _pdfViewerService.SaveDocumentAsync(
                PdfViewerControl,
                _currentFilePath,
                flatten);

            if (result.Success)
            {
                _hasUnsavedChanges = false;
                StatusLabel.Text = flatten
                    ? "Document flattened and saved successfully."
                    : "Document saved successfully.";
            }
            else
            {
                StatusLabel.Text = $"Save failed: {result.ErrorMessage}";
            }

            return result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PdfViewerPage] SaveDocumentAsync failed: {ex}");
            StatusLabel.Text = $"Save failed: {ex.Message}";
            return new PdfViewerResult(false, ex.Message, PdfOperationReason.Save);
        }
    }

    /// <summary>
    /// Exports the PDF to a new file path with optional flattening.
    /// Useful for "Save As" workflows.
    /// </summary>
    public async Task<PdfViewerResult> ExportDocumentAsync(string destinationPath, bool flatten)
    {
        if (string.IsNullOrEmpty(destinationPath))
        {
            return new PdfViewerResult(false, "No destination path specified.", PdfOperationReason.Export);
        }

        StatusLabel.Text = flatten ? "Exporting with flattening..." : "Exporting...";

        try
        {
            var result = await _pdfViewerService.ExportDocumentAsync(
                PdfViewerControl,
                destinationPath,
                flatten);

            if (result.Success)
            {
                StatusLabel.Text = flatten
                    ? "Document flattened and exported successfully."
                    : "Document exported successfully.";
            }
            else
            {
                StatusLabel.Text = $"Export failed: {result.ErrorMessage}";
            }

            return result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PdfViewerPage] ExportDocumentAsync failed: {ex}");
            StatusLabel.Text = $"Export failed: {ex.Message}";
            return new PdfViewerResult(false, ex.Message, PdfOperationReason.Export);
        }
    }

    /// <summary>
    /// Navigates to a specific page in the document.
    /// </summary>
    public void GoToPage(int pageNumber)
    {
        if (pageNumber >= 1 && pageNumber <= PdfViewerControl.PageCount)
        {
            PdfViewerControl.GoToPage(pageNumber);
            StatusLabel.Text = $"Page {pageNumber} of {PdfViewerControl.PageCount}";
        }
    }

    /// <summary>
    /// Gets whether there are unsaved changes in the current document.
    /// </summary>
    public bool HasUnsavedChanges => _hasUnsavedChanges;

    /// <summary>
    /// Gets the current page number being viewed.
    /// </summary>
    public int CurrentPageNumber => PdfViewerControl.PageNumber;

    private async void OnImportAnnotationsClicked(object sender, EventArgs e)
    {
        await ImportAnnotationsAsync();
    }

    private async void OnExportAnnotationsClicked(object sender, EventArgs e)
    {
        await ExportAnnotationsAsync();
    }

    private async void OnImportFormDataClicked(object sender, EventArgs e)
    {
        await ImportFormDataAsync();
    }

    private async void OnExportFormDataClicked(object sender, EventArgs e)
    {
        await ExportFormDataAsync();
    }

    /// <summary>
    /// Imports annotations from an XFDF file using file picker dialog.
    /// Supports XFDF, FDF, and JSON formats.
    /// </summary>
    private async Task ImportAnnotationsAsync()
    {
        if (PdfViewerControl == null)
        {
            await DisplayAlertAsync("Error", "PDF viewer not initialized.", "OK");
            return;
        }

        try
        {
            StatusLabel.Text = "Selecting annotation file to import...";

            // Use FilePicker to let user select annotation file
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                
            });

            if (result == null)
            {
                StatusLabel.Text = "Import cancelled.";
                return;
            }

            // Determine file format based on extension
            string extension = Path.GetExtension(result.FileName).ToLower();
            AnnotationDataFormat format = extension switch
            {
                ".xfdf" => AnnotationDataFormat.XFdf,
                ".fdf" => AnnotationDataFormat.Fdf,
                ".json" => AnnotationDataFormat.Json,
                _ => AnnotationDataFormat.XFdf // Default to XFDF
            };

            StatusLabel.Text = $"Importing annotations from {Path.GetFileName(result.FileName)}...";

            // Open file stream and import annotations
            using (var stream = await result.OpenReadAsync())
            {
                stream.Position = 0;
                await PdfViewerControl.ImportAnnotationsAsync(stream, format);
            }

            _hasUnsavedChanges = true;
            StatusLabel.Text = $"✓ Annotations imported from {Path.GetFileName(result.FileName)}";
#if NET10_0_OR_GREATER
            await DisplayAlertAsync("Success", $"Annotations imported successfully.\nFile: {Path.GetFileName(result.FileName)}", "OK");
#else
            await DisplayAlert("Success", $"Annotations imported successfully.\nFile: {Path.GetFileName(result.FileName)}", "OK");
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PdfViewerPage] ImportAnnotationsAsync failed: {ex}");
            StatusLabel.Text = $"Import failed: {ex.Message}";
#if NET10_0_OR_GREATER
            await DisplayAlertAsync("Error", $"Failed to import annotations: {ex.Message}", "OK");
#else
            await DisplayAlert("Error", $"Failed to import annotations: {ex.Message}", "OK");
#endif
        }
    }

    /// <summary>
    /// Exports all annotations to an XFDF file using file picker dialog.
    /// Supports XFDF, FDF, and JSON formats.
    /// </summary>
    private async Task ExportAnnotationsAsync()
    {
        if (PdfViewerControl == null)
        {
            await DisplayAlertAsync("Error", "PDF viewer not initialized.", "OK");
            return;
        }

        try
        {
            StatusLabel.Text = "Selecting location to save annotations...";

            // Use FileService to show save dialog
            // Default format is XFDF
            string suggestedFileName = string.IsNullOrEmpty(_currentFileName)
                ? "annotations.xfdf"
                : Path.GetFileNameWithoutExtension(_currentFileName) + "_annotations.xfdf";

            // Create a memory stream for the annotation data
            var annotationStream = new MemoryStream();

            // Export annotations to memory stream
            await PdfViewerControl.ExportAnnotationsAsync(annotationStream, AnnotationDataFormat.XFdf);
            annotationStream.Position = 0;

            // Use FileService to save the annotation file
            string? filePath = await FileService.SaveAsAsync(suggestedFileName, annotationStream);

            if (!string.IsNullOrEmpty(filePath))
            {
                StatusLabel.Text = $"✓ Annotations exported to {Path.GetFileName(filePath)}";
#if NET10_0_OR_GREATER
                await DisplayAlertAsync("Success", $"Annotations exported successfully.\nFile: {Path.GetFileName(filePath)}", "OK");
#else
                await DisplayAlert("Success", $"Annotations exported successfully.\nFile: {Path.GetFileName(filePath)}", "OK");
#endif
            }
            else
            {
                StatusLabel.Text = "Export cancelled.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PdfViewerPage] ExportAnnotationsAsync failed: {ex}");
            StatusLabel.Text = $"Export failed: {ex.Message}";
#if NET10_0_OR_GREATER
            await DisplayAlertAsync("Error", $"Failed to export annotations: {ex.Message}", "OK");
#else
            await DisplayAlert("Error", $"Failed to export annotations: {ex.Message}", "OK");
#endif
        }
    }

    /// <summary>
    /// Imports form field data from an XFDF/FDF/JSON file using file picker dialog.
    /// Supports XFDF, FDF, JSON, and XML formats.
    /// </summary>
    private async Task ImportFormDataAsync()
    {
        if (PdfViewerControl == null)
        {
            await DisplayAlertAsync("Error", "PDF viewer not initialized.", "OK");
            return;
        }

        try
        {
            StatusLabel.Text = "Selecting form data file to import...";

            // Use FilePicker to let user select form data file
            var result = await FilePicker.Default.PickAsync();

            if (result == null)
            {
                StatusLabel.Text = "Import cancelled.";
                return;
            }

            // Determine file format based on extension
            string extension = Path.GetExtension(result.FileName).ToLower();
            DataFormat format = extension switch
            {
                ".xfdf" => DataFormat.XFdf,
                ".fdf" => DataFormat.Fdf,
                ".json" => DataFormat.Json,
                ".xml" => DataFormat.Xml,
                _ => DataFormat.XFdf // Default to XFDF
            };

            StatusLabel.Text = $"Importing form data from {Path.GetFileName(result.FileName)}...";

            // Open file stream and import form data
            using (var stream = await result.OpenReadAsync())
            {
                stream.Position = 0;
                PdfViewerControl.ImportFormData(stream, format);
            }

            _hasUnsavedChanges = true;
            StatusLabel.Text = $"✓ Form data imported from {Path.GetFileName(result.FileName)}";
#if NET10_0_OR_GREATER
            await DisplayAlertAsync("Success", $"Form data imported successfully.\nFile: {Path.GetFileName(result.FileName)}", "OK");
#else
            await DisplayAlert("Success", $"Form data imported successfully.\nFile: {Path.GetFileName(result.FileName)}", "OK");
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PdfViewerPage] ImportFormDataAsync failed: {ex}");
            StatusLabel.Text = $"Import failed: {ex.Message}";
#if NET10_0_OR_GREATER
            await DisplayAlertAsync("Error", $"Failed to import form data: {ex.Message}", "OK");
#else
            await DisplayAlert("Error", $"Failed to import form data: {ex.Message}", "OK");
#endif
        }
    }

    /// <summary>
    /// Exports all form field data to an XFDF file using file picker dialog.
    /// Supports XFDF, FDF, JSON, and XML formats.
    /// </summary>
    private async Task ExportFormDataAsync()
    {
        if (PdfViewerControl == null)
        {
            await DisplayAlertAsync("Error", "PDF viewer not initialized.", "OK");
            return;
        }

        try
        {
            StatusLabel.Text = "Selecting location to save form data...";

            // Use FileService to show save dialog
            // Default format is XFDF
            string suggestedFileName = string.IsNullOrEmpty(_currentFileName)
                ? "formdata.xfdf"
                : Path.GetFileNameWithoutExtension(_currentFileName) + "_formdata.xfdf";

            // Create a memory stream for the form data
            var formDataStream = new MemoryStream();

            // Export form data to memory stream
            PdfViewerControl.ExportFormData(formDataStream, DataFormat.XFdf);
            formDataStream.Position = 0;

            // Use FileService to save the form data file
            string? filePath = await FileService.SaveAsAsync(suggestedFileName, formDataStream);

            if (!string.IsNullOrEmpty(filePath))
            {
                StatusLabel.Text = $"✓ Form data exported to {Path.GetFileName(filePath)}";
#if NET10_0_OR_GREATER
                await DisplayAlertAsync("Success", $"Form data exported successfully.\nFile: {Path.GetFileName(filePath)}", "OK");
#else
                await DisplayAlert("Success", $"Form data exported successfully.\nFile: {Path.GetFileName(filePath)}", "OK");
#endif
            }
            else
            {
                StatusLabel.Text = "Export cancelled.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PdfViewerPage] ExportFormDataAsync failed: {ex}");
            StatusLabel.Text = $"Export failed: {ex.Message}";
#if NET10_0_OR_GREATER
            await DisplayAlertAsync("Error", $"Failed to export form data: {ex.Message}", "OK");
#else
            await DisplayAlert("Error", $"Failed to export form data: {ex.Message}", "OK");
#endif
        }
    }
}
