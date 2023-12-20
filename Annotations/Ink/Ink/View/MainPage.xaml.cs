using Syncfusion.Maui.PdfViewer;


namespace Ink;

public partial class MainPage : ContentPage
{
    Annotation SelectedAnnotation;
    //When loading password protected files, it is used to wait the current thread's execution, until the user enters the password.
    private ManualResetEvent manualResetEvent = new ManualResetEvent(false);

    public MainPage()
    {
        InitializeComponent();
        inkEditor.SelectedThickness = PdfViewer.AnnotationSettings.Ink.BorderWidth;
        inkEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Ink.Color.Alpha;
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedThickness = inkEditor.SelectedThickness;
            viewModel.SelectedOpacity = inkEditor.SelectedOpacity;
        }
        string[,] colorCodes = new string[1, 5] {
            { "#FF990000", "#FF996100", "#FF009907", "#FF060099", "#FF990098"},
        };
        inkEditor.ThicknessChanged += FreeTextEditor_BorderThicknessChanged;
        inkEditor.ColorChanged += FreeTextEditor_FontColorChanged;
        inkEditor.OpacityChanged += FreeTextEditor_OpacityChanged;
        PdfViewer.AnnotationSelected += PdfViewer_AnnotationSelected;
        PdfViewer.AnnotationDeselected += PdfViewer_AnnotationDeselected;
#if ANDROID
    Export.Margin = new Thickness(1,0,0,0);
#elif MACCATALYST
    Save.Margin = new Thickness(4,0,4,0);
#elif WINDOWS
    Save.Margin = new Thickness(4,0,9,0);
#endif
    }

    private void FreeTextEditor_OpacityChanged(object? sender, float e)
    {
        if (SelectedAnnotation != null && SelectedAnnotation is InkAnnotation inkAnnotation)
            inkAnnotation.Opacity = e;
        else
            PdfViewer.AnnotationSettings.Ink.Opacity = e;
    }

    private void FreeTextEditor_FontColorChanged(object? sender, Color e)
    {
        if (SelectedAnnotation != null && SelectedAnnotation is InkAnnotation inkAnnotation)
            inkAnnotation.Color = e;
        else
            PdfViewer.AnnotationSettings.Ink.Color = e;
    }

    private void FreeTextEditor_BorderThicknessChanged(object? sender, float e)
    {
        if (SelectedAnnotation != null && SelectedAnnotation is InkAnnotation inkAnnotation)
            inkAnnotation.BorderWidth = e;
        else
            PdfViewer.AnnotationSettings.Ink.BorderWidth = e;
    }

    /// <summary>
    /// Event handler for the "AnnotationDeselected" event of the PdfViewer.
    /// Clears the selected annotation and adjusts the visibility and layout of the EditorControl and associated UI elements.
    /// </summary>
    private void PdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = null;
        inkEditor.IsVisible = false;
        Delete.IsVisible = false;
        AnnotationPropertyGrid.IsVisible = false;
    }

    /// <summary>
    /// Event handler for the "AnnotationSelected" event of the PdfViewer.
    /// Handles the selection of an annotation by updating UI elements in the EditorControl based on the selected annotation type.
    /// </summary>
    private void PdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = e.Annotation;
        if(SelectedAnnotation is InkAnnotation inkAnnotation)
        {
            inkEditor.SelectedThickness = inkAnnotation.BorderWidth;
            inkEditor.SelectedOpacity = inkAnnotation.Opacity;
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedThickness = inkAnnotation.BorderWidth;
                viewModel.SelectedOpacity = inkAnnotation.Opacity;
            }
        }
        Delete.IsVisible = true;
        if (!AnnotationPropertyGrid.IsVisible)
        {
            AnnotationPropertyGrid.IsVisible = true;
        }
        UnlockButton.IsVisible = SelectedAnnotation.IsLocked ? true : false;
        Lockbutton.IsVisible = SelectedAnnotation.IsLocked ? false : true;
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


    private void ShowPropertyPanel_Clicked(object sender, EventArgs e)
    {
        if(PdfViewer.AnnotationMode == AnnotationMode.Ink)
        {
            PdfViewer.AnnotationMode = AnnotationMode.None;
            AnnotationPropertyGrid.IsVisible = false;
            inkEditor.IsVisible = false;
            Delete.IsVisible = false;
        }
        else
        {
            PdfViewer.AnnotationMode = AnnotationMode.Ink;
            AnnotationPropertyGrid.IsVisible = true;
        }
    }

    /// <summary>
    /// Handles the event when the color palette button is clicked, toggling the visibility of the editor control.
    /// </summary>
    private void ColorPalette_Clicked(object sender, EventArgs e)
    {
        inkEditor.IsVisible = !inkEditor.IsVisible;
    }

    /// <summary>
    /// Handles the event when an annotation is deselected in the PdfViewer.
    /// </summary>
    private void Delete_Clicked(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            PdfViewer.RemoveAnnotation(SelectedAnnotation);
        }
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
        UnlockButton.IsVisible = SelectedAnnotation.IsLocked ? true : false;
        Lockbutton.IsVisible = SelectedAnnotation.IsLocked ? false : true;
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


    /// <summary>
    /// Handles the event when the "Import" button is clicked, importing annotations from an XFDF file.
    /// </summary>
    private async void Import_Clicked(object sender, EventArgs e)
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
    public async Task CopyFileToAppDataDirectory(Stream inputStream, string filename)
    {
        // Create an output filename
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
        // Copy the file to the AppDataDirectory
        using FileStream outputStream = File.Create(targetFile);
        await inputStream.CopyToAsync(outputStream);
        await DisplayAlert("Annotations exported", "The annotations are exported to the file " + targetFile, "OK");
    }
}