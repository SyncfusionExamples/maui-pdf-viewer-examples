using Microsoft.Maui;
using Microsoft.Maui.Platform;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Maui.Sliders;
using System.Windows.Input;

namespace FreeText;

public partial class MainPage : ContentPage
{
   
    Annotation SelectedAnnotation;
    public MainPage()
    {
        InitializeComponent();
        string[,] colorCodes = new string[1, 5] {
            { "#FF990000", "#FF996100", "#FF009907", "#FF060099", "#FF990098"},
        };
        freeTextEditor.SelectedThickness = PdfViewer.AnnotationSettings.FreeText.BorderWidth;
        freeTextEditor.SelectedOpacity = PdfViewer.AnnotationSettings.FreeText.Color.Alpha;
        freeTextEditor.SelectedFontSize = (float)PdfViewer.AnnotationSettings.FreeText.FontSize;
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedThickness = freeTextEditor.SelectedThickness;
            viewModel.SelectedOpacity = freeTextEditor.SelectedOpacity;
            viewModel.SelectedFontSize = freeTextEditor.SelectedFontSize;
        }
        freeTextEditor.FillColorChanged += FreeTextEditor_FillColorChanged;
        freeTextEditor.BorderThicknessChanged += FreeTextEditor_BorderThicknessChanged;
        freeTextEditor.BorderColorChanged += FreeTextEditor_BorderColorChanged;
        freeTextEditor.FontColorChanged += FreeTextEditor_FontColorChanged;
        freeTextEditor.OpacityChanged += FreeTextEditor_OpacityChanged;
        freeTextEditor.FontSizeChanged += FreeTextEditor_FontSizeChanged;
        PdfViewer.AnnotationSelected += PdfViewer_AnnotationSelected;
        PdfViewer.AnnotationDeselected += PdfViewer_AnnotationDeselected;
#if ANDROID
    Export.Margin = new Thickness(1,0,0,0);
#elif MACCATALYST
    Save.Margin = new Thickness(4,0,4,0);
#elif WINDOWS
        Save.Margin = new Thickness(4, 0, 9, 0);
#endif
    }

    private void FreeTextEditor_FontSizeChanged(object? sender, double e)
    {
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedFontSize = (float)e;
        }
        if (SelectedAnnotation != null && SelectedAnnotation is FreeTextAnnotation freeTextAnnotation)
            freeTextAnnotation.FontSize = e;
        else
            PdfViewer.AnnotationSettings.FreeText.FontSize = e;
    }

    private void FreeTextEditor_OpacityChanged(object? sender, float e)
    {
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedOpacity = (float)e;
        }
        if (SelectedAnnotation != null && SelectedAnnotation is FreeTextAnnotation freeTextAnnotation)
            freeTextAnnotation.Opacity = e;
        else
            PdfViewer.AnnotationSettings.FreeText.Opacity = e;
    }

    private void FreeTextEditor_FontColorChanged(object? sender, Color e)
    {
        if (SelectedAnnotation != null && SelectedAnnotation is FreeTextAnnotation freeTextAnnotation)
            freeTextAnnotation.Color = e;
        else
            PdfViewer.AnnotationSettings.FreeText.Color = e;
    }

    private void FreeTextEditor_BorderColorChanged(object? sender, Color e)
    {
        if (SelectedAnnotation != null && SelectedAnnotation is FreeTextAnnotation freeTextAnnotation)
            freeTextAnnotation.BorderColor = e;
        else
            PdfViewer.AnnotationSettings.FreeText.BorderColor = e;
    }

    private void FreeTextEditor_BorderThicknessChanged(object? sender, float e)
    {
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedThickness = (float)e;
        }
        if (SelectedAnnotation != null && SelectedAnnotation is FreeTextAnnotation freeTextAnnotation)
            freeTextAnnotation.BorderWidth = e;
        else
            PdfViewer.AnnotationSettings.FreeText.BorderWidth = e;
    }

    private void FreeTextEditor_FillColorChanged(object? sender, Color e)
    {
        if(SelectedAnnotation != null)
        {
            SelectedAnnotation.FillColor = e;
        }
        else
        {
            PdfViewer.AnnotationSettings.FreeText.FillColor = e;
        }
    }

    /// <summary>
    /// Handles the deselection of an annotation, resetting the UI and hiding the editor controls.
    /// </summary>
    private void PdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = null;
        ColorPalatte.IsVisible = false;
        freeTextEditor.IsVisible = false;
        delete.IsVisible = false;
        Lock.IsVisible = false;
        Unlock.IsVisible = false;
        
    }

    /// <summary>
    /// Handles the selection of an annotation, updating the UI to display the appropriate editor controls and options.
    /// </summary>
    private void PdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = e.Annotation;
        delete.IsVisible = true;
        if (SelectedAnnotation is FreeTextAnnotation freeText)
        {
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedThickness = freeText.BorderWidth;
                viewModel.SelectedOpacity = SelectedAnnotation.Opacity;
                viewModel.SelectedFontSize = (float)freeText.FontSize;
            }
            ColorPalatte.IsVisible = true;
        }
        Lock.IsVisible = true;
        Unlock.IsVisible = true;

    }
   
    /// <summary>
    /// Handles the button enable and disable states when the button property changes.
    /// </summary>
    private void Button_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is Button button)
        {
            if (e.PropertyName == "IsEnabled")
            {
                if (!button.IsEnabled)
                {
                    button.Unfocus();
                }
                else
                {
                    Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(1), () =>
                    {
                        VisualStateManager.GoToState(button, "Normal");
                    });
                }
            }
        }
    }

    /// <summary>
    /// Event handler for the annotation menu button click.
    /// Manages the visibility of annotation-related UI elements and updates the PDF viewer's annotation mode.
    /// </summary>
    private void AnnotationMenu_Clicked(object sender, EventArgs e)
    {
        if (PdfViewer.AnnotationMode == AnnotationMode.FreeText)
        {
            PdfViewer.AnnotationMode = AnnotationMode.None;
            ColorPalatte.IsVisible = false;
            freeTextEditor.IsVisible = false;
        }
        else
        {
            PdfViewer.AnnotationMode = AnnotationMode.FreeText;
            ColorPalatte.IsVisible = true;
        }
    }

    /// <summary>
    /// Handles the event when the "Delete" button is clicked, removing the selected annotation from the PdfViewer.
    /// </summary>
    private void Delete_Clicked(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            PdfViewer.RemoveAnnotation(SelectedAnnotation);
        }
    }

    /// <summary>
    /// Handles the event when the "Save" button is clicked, saving the PDF document to a file in the common app data directory.
    /// </summary>
    private async void Save_Clicked(object sender, EventArgs e)
    {
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "Saved.pdf");
        using FileStream outputStream = File.Create(targetFile);
        await PdfViewer.SaveDocumentAsync(outputStream);
        await DisplayAlert("Document Saved", "The document has been saved to the file " + targetFile, "OK");
    }

    private void Lock_Clicked(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.IsLocked = true;
        }
    }

    private void Unlock_Clicked(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.IsLocked = false;
        }
    }

    /// <summary>
    /// Handles the event when the "Export" button is clicked, exporting annotations to an XFDF file.
    /// </summary>
    private async void Export_Clicked(object sender, EventArgs e)
    {
        Stream xfdfStream = new MemoryStream();
        await PdfViewer.ExportAnnotationsAsync(xfdfStream, Syncfusion.Pdf.Parsing.AnnotationDataFormat.XFdf);
        await CopyFileToAppDataDirectory(xfdfStream, "Export.xfdf");
    }

    /// <summary>
    /// Copies the contents of the input stream to a file in the application's data directory.
    /// </summary>
    /// <param name="inputStream">The input stream containing the data to be copied.</param>
    /// <param name="filename">The name of the target file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task CopyFileToAppDataDirectory(Stream inputStream, string fileName)
    {
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        using FileStream outputStream = File.Create(targetFile);
        await inputStream.CopyToAsync(outputStream);
        await DisplayAlert("Annotations Exported", "The annotations are exported to the file " + targetFile  , " OK");
    }

    /// <summary>
    /// Handles the event when the "Lock/Unlock" button is clicked, toggling the lock status of the selected annotation.
    /// </summary>
    private void LockUnlock_Clicked(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.IsLocked = !SelectedAnnotation.IsLocked;
        }
        Unlock.IsVisible = SelectedAnnotation.IsLocked;
        Lock.IsVisible = !SelectedAnnotation.IsLocked;
    }

    private async void  Import_Clicked(object sender, EventArgs e)
    {
        string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "Export.xfdf");
        if (File.Exists(fileName))
        {
            Stream inputStream = File.OpenRead(fileName);
            inputStream.Position = 0;
            await PdfViewer.ImportAnnotationsAsync(inputStream, Syncfusion.Pdf.Parsing.AnnotationDataFormat.XFdf);
            await DisplayAlert("Information", "Annotations Loaded from the " + fileName, "OK");
        }
        else
            await DisplayAlert("XFDF file Not Found", "No xfdf files available for import. Please export the annotations to an xfdf file and then import. ", "OK");
    }

    private void ColorPalatte_Clicked(object sender, EventArgs e)
    {
        freeTextEditor.IsVisible = !freeTextEditor.IsVisible;
    }
}