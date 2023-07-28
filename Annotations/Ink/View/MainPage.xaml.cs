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
        string[,] colorCodes = new string[1, 5] {
            { "#FF990000", "#FF996100", "#FF009907", "#FF060099", "#FF990098"},
        };
        EditorControl.ConfigureColorPicker(colorCodes);
        EditorControl.ConfigureOpacity(1);
        EditorControl.ConfigureThickness();
        EditorControl.ColorChanged += EditorControl_ColorChanged;
        EditorControl.OpacityChangedEnd += EditorControl_OpacityChangedEnd;
        EditorControl.ThicknessChangedEnd += EditorControl_ThicknessChangedEnd;
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

    /// <summary>
    /// Event handler for the "ThicknessChangedEnd" event of the EditorControl.
    /// Updates the border width of the selected annotation or the default ink annotation in the PdfViewer based on the EditorThickness value.
    /// </summary>
    
    private void EditorControl_ThicknessChangedEnd(object sender, EventArgs e)
    {
        if(SelectedAnnotation != null)
        {
            if(SelectedAnnotation is  ShapeAnnotation shape)
            {
                shape.BorderWidth = (int)EditorControl.EditorThickness;
            }
            if (SelectedAnnotation is InkAnnotation ink)
            {
                ink.BorderWidth = (int)EditorControl.EditorThickness;
            }
        }
        else
        {
            PdfViewer.AnnotationSettings.Ink.BorderWidth = (int)EditorControl.EditorThickness;
        }
    }

    /// <summary>
    /// Event handler for the "OpacityChangedEnd" event of the EditorControl.
    /// Updates the opacity of the selected annotation or the default ink annotation in the PdfViewer based on the EditorOpacity value.
    /// </summary>
   
    private void EditorControl_OpacityChangedEnd(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Opacity = (float)EditorControl.EditorOpacity;
            PdfViewer.AnnotationSettings.Ink.Opacity = (float)EditorControl.EditorOpacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Ink)
        {
            PdfViewer.AnnotationSettings.Ink.Opacity = (float)EditorControl.EditorOpacity;
        }
    }

    /// <summary>
    /// Event handler for the "ColorChanged" event of the EditorControl.
    /// Updates the color of the selected annotation or the default ink annotation in the PdfViewer based on the ColorChangedEventArgs.
    /// </summary>

    private void EditorControl_ColorChanged(object sender, ColorChangedEventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Color = Color.FromArgb(e.ColorCode);
        }
        else if(PdfViewer.AnnotationMode == AnnotationMode.Ink)
        {
            PdfViewer.AnnotationSettings.Ink.Color = Color.FromArgb(e.ColorCode);
        }
    }

    /// <summary>
    /// Event handler for the "AnnotationDeselected" event of the PdfViewer.
    /// Clears the selected annotation and adjusts the visibility and layout of the EditorControl and associated UI elements.
    /// </summary>
   
    private void PdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = null;
        EditorControl.IsVisible = false;
        AnnotationPropertyGrid.IsVisible = false;
        ColorPalletteReset();
    }

    /// <summary>
    /// Event handler for the "AnnotationSelected" event of the PdfViewer.
    /// Handles the selection of an annotation by updating UI elements in the EditorControl based on the selected annotation type.
    /// </summary>
    
    private void PdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = e.Annotation;
        EditorControl.EditorOpacity = SelectedAnnotation.Opacity;
        if (!AnnotationPropertyGrid.IsVisible)
        {
            AnnotationPropertyGrid.IsVisible = true;
        }
        UnlockButton.IsVisible = SelectedAnnotation.IsLocked ? true : false;
        Lockbutton.IsVisible = SelectedAnnotation.IsLocked ? false : true;
        if (SelectedAnnotation is UnderlineAnnotation || SelectedAnnotation is StrikeOutAnnotation ||
                SelectedAnnotation is SquigglyAnnotation ||
                SelectedAnnotation is HighlightAnnotation)
        {
            EditorControl.ThicknessSliderLayOut.IsVisible = false;
            EditorControl.ColorPaletteLayOut.IsVisible = true;
            EditorControl.ColorOpacitySeparator.IsVisible = true;
            EditorControl.OpacityThicknessSeparator.IsVisible = false;
            EditorControl.HeightRequest = 123f;
        }
        else if (SelectedAnnotation is StampAnnotation)
        {
            EditorControl.ColorPaletteLayOut.IsVisible = false;
            EditorControl.ThicknessSliderLayOut.IsVisible = false;
            EditorControl.ColorOpacitySeparator.IsVisible = false;
            EditorControl.OpacityThicknessSeparator.IsVisible = false;
            EditorControl.HeightRequest = 80f;
        }
        else
        {
            ColorPalletteReset();
        }
        if (SelectedAnnotation is ShapeAnnotation shape)
        {
            EditorControl.EditorThickness = shape.BorderWidth;
        }
        else if (SelectedAnnotation is InkAnnotation ink)
        {
            EditorControl.EditorThickness = ink.BorderWidth;
        }
    }

    /// <summary>
    /// Event handler for the "PropertyChanged" event of the EditOption.
    /// Hides the EditorControl if the "IsVisible" property of the EditOption is changed.
    /// </summary>
    
    private void EditOption_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible))
        {
            EditorControl.IsVisible = false;
        }
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


    private void ColorPalletteReset()
    {
        EditorControl.ThicknessSliderLayOut.IsVisible = true;
        EditorControl.ColorPaletteLayOut.IsVisible = true;
        EditorControl.ColorOpacitySeparator.IsVisible = true;
        EditorControl.OpacityThicknessSeparator.IsVisible = true;
        EditorControl.HeightRequest = 200f;
    }


    private void ShowPropertyPanel_Clicked(object sender, EventArgs e)
    {
        if(PdfViewer.AnnotationMode == AnnotationMode.Ink)
        {
            PdfViewer.AnnotationMode = AnnotationMode.None;
            EditorControl.IsVisible = false;
        }
        else
        {
            PdfViewer.AnnotationMode = AnnotationMode.Ink;
            EditorControl.EditorOpacity = PdfViewer.AnnotationSettings.Ink.Opacity;
            EditorControl.EditorThickness = PdfViewer.AnnotationSettings.Ink.BorderWidth;
            EditorControl.IsVisible = true;
        }
    }

    /// <summary>
    /// Handles the event when the color palette button is clicked, toggling the visibility of the editor control.
    /// </summary>
    private void ColorPalette_Clicked(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            EditorControl.IsVisible = !EditorControl.IsVisible;
        }
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
        PdfViewer.SaveDocument(outputStream);
        await DisplayAlert("Document Saved", "The document has been saved to the file " + targetFile, "OK");
    }


    /// <summary>
    /// Handles the event when the "Import" button is clicked, importing annotations from an XFDF file.
    /// </summary>
    private void Import_Clicked(object sender, EventArgs e)
    {
        string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "Export.xfdf");
        if (File.Exists(fileName))
        {
            Stream inputStream = File.OpenRead(fileName);
            inputStream.Position = 0;
            PdfViewer.ImportAnnotations(inputStream, Syncfusion.Pdf.Parsing.AnnotationDataFormat.XFdf);
            DisplayAlert("Annotations imported", "Annotations from the " + fileName + " file are imported", "OK");
        }
        else
            DisplayAlert("No files to import", "There are no xfdf files to import. Please export the annotations to an xfdf file and then import. ", "OK");
    }

    /// <summary>
    /// Handles the event when the "Export" button is clicked, exporting annotations to an XFDF file.
    /// </summary>
    private async void Export_Clicked(object sender, EventArgs e)
    {
        Stream xfdfStream = new MemoryStream();
        PdfViewer.ExportAnnotations(xfdfStream, Syncfusion.Pdf.Parsing.AnnotationDataFormat.XFdf);
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