using Microsoft.Maui.Platform;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Maui.Sliders;
using System.Windows.Input;

namespace Shapes;

public partial class MainPage : ContentPage
{
   
    Annotation SelectedAnnotation;
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
        Save.Margin = new Thickness(4, 0, 9, 0);
#endif
    }

    /// <summary>
    /// Updates the opacity of the selected annotation or the default annotation settings based on the EditorControl's opacity change.
    /// </summary>
    private void EditorControl_OpacityChangedEnd(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Opacity = (float)EditorControl.EditorOpacity;
        }
        else
        {
            PdfViewer.AnnotationSettings.Arrow.Opacity = (float)EditorControl.EditorOpacity;
            PdfViewer.AnnotationSettings.Line.Opacity = (float)EditorControl.EditorOpacity;
            PdfViewer.AnnotationSettings.Square.Opacity= (float)EditorControl.EditorOpacity;
            PdfViewer.AnnotationSettings.Circle.Opacity= (float)EditorControl.EditorOpacity;
            PdfViewer.AnnotationSettings.Ink.Opacity= (float)EditorControl.EditorOpacity;
        }
    }

    /// <summary>
    /// Updates the color of the selected annotation or the default annotation settings based on the EditorControl's color change.
    /// </summary>
    private void EditorControl_ColorChanged(object sender, ColorChangedEventArgs e)
    {
        
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Color = Color.FromArgb(e.ColorCode);
        }
        else
        {
            PdfViewer.AnnotationSettings.Arrow.Color = Color.FromArgb(e.ColorCode);
            PdfViewer.AnnotationSettings.Line.Color = Color.FromArgb(e.ColorCode);
            PdfViewer.AnnotationSettings.Square.Color = Color.FromArgb(e.ColorCode);
            PdfViewer.AnnotationSettings.Circle.Color = Color.FromArgb(e.ColorCode);
            PdfViewer.AnnotationSettings.Ink.Color = Color.FromArgb(e.ColorCode);

        }
    }

    /// <summary>
    /// Handles the deselection of an annotation, resetting the UI and hiding the editor controls.
    /// </summary>
    private void PdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = null;
        EditorControl.IsVisible = false;
        AnnotationIcons.IsVisible=true;
        Delete.IsVisible = false;
        Unlock.IsVisible = false;
        Lock.IsVisible = false;
        EditorControl.ThicknessSliderLayOut.IsVisible = true;
        EditorControl.ColorPaletteLayOut.IsVisible = true;
        EditorControl.OpacityThicknessSeperator.IsVisible = true;
        EditorControl.ColorOpacitySeperator.IsVisible = true;
        EditorControl.HeightRequest = 200f;
        
    }

    /// <summary>
    /// Handles the selection of an annotation, updating the UI to display the appropriate editor controls and options.
    /// </summary>
    private void PdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = e.Annotation;  
        AnnotationMenuGrid.IsVisible = true;
        AnnotationIcons.IsVisible = false;
        Delete.IsVisible= true;
        ColorPalette.IsVisible = true;
        if (SelectedAnnotation is UnderlineAnnotation  || SelectedAnnotation is StrikeOutAnnotation  ||
                SelectedAnnotation is SquigglyAnnotation  ||
                SelectedAnnotation is HighlightAnnotation )
        {
            EditorControl.EditorOpacity=SelectedAnnotation.Opacity;
            EditorControl.ThicknessSliderLayOut.IsVisible = false;
            EditorControl.ColorPaletteLayOut.IsVisible = true;
            EditorControl.OpacityThicknessSeperator.IsVisible = false;
            EditorControl.ColorOpacitySeperator.IsVisible = true;
            EditorControl.HeightRequest = 123f;
        }
        else if (SelectedAnnotation is StampAnnotation stamp)
        {
            EditorControl.EditorOpacity=stamp.Opacity;
            EditorControl.ColorPaletteLayOut.IsVisible = false;
            EditorControl.ThicknessSliderLayOut.IsVisible = false;
            EditorControl.OpacityThicknessSeperator.IsVisible = false;
            EditorControl.ColorOpacitySeperator.IsVisible = false;
            EditorControl.HeightRequest = 90f;
        }
        else
        {
            EditorControl.ThicknessSliderLayOut.IsVisible = true;
            EditorControl.ColorPaletteLayOut.IsVisible = true;
            EditorControl.ColorOpacitySeperator.IsVisible = true;
            EditorControl.OpacityThicknessSeperator.IsVisible = true;
            
            EditorControl.HeightRequest = 200f;
        }
        if (SelectedAnnotation is ShapeAnnotation shape)
        {
            EditorControl.EditorThickness = shape.BorderWidth;
            EditorControl.EditorOpacity= shape.Opacity;
        }
        else if (SelectedAnnotation is InkAnnotation ink)
        {
            EditorControl.EditorThickness = ink.BorderWidth;
            EditorControl.EditorOpacity= ink.Opacity;
        }
      
        if (SelectedAnnotation.IsLocked)
        {
            Unlock.IsVisible = true;
            Lock.IsVisible = false;
        }
        else
        {
            Unlock.IsVisible = false;
            Lock.IsVisible = true;
        }


    }

    /// <summary>
    /// Updates the thickness (border width) of the selected annotation or the default annotation settings based on the EditorControl's thickness change.
    /// </summary>
    private void EditorControl_ThicknessChangedEnd(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            if(SelectedAnnotation is ShapeAnnotation shape)
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
            PdfViewer.AnnotationSettings.Arrow.BorderWidth = (int)EditorControl.EditorThickness;
            PdfViewer.AnnotationSettings.Line.BorderWidth = (int)EditorControl.EditorThickness;
            PdfViewer.AnnotationSettings.Square.BorderWidth = (int)EditorControl.EditorThickness;
            PdfViewer.AnnotationSettings.Circle.BorderWidth = (int)EditorControl.EditorThickness;
            PdfViewer.AnnotationSettings.Ink.BorderWidth = (int)EditorControl.EditorThickness;
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

    /// <summary>
    /// Event handler for the annotation buttons' click events.
    /// Sets the PDF viewer's annotation mode based on the clicked button.
    /// </summary>
    private void Annotation_Clicked(object sender, EventArgs e)
    {
        
            if (sender is Button clickedButton)
            {
                // Check which button is clicked based on its x:Name property
                if (clickedButton == Arrow)
                {
                // Handle Arrow button click
                    PdfViewer.AnnotationMode = AnnotationMode.Arrow;
                }
                else if (clickedButton == Line)
                {
                // Handle Line button click
                    PdfViewer.AnnotationMode = AnnotationMode.Line;
                }
                else if (clickedButton == Rectangle)
                {
                // Handle Rectangle button click
                    PdfViewer.AnnotationMode = AnnotationMode.Square;
                }
                else if (clickedButton == Circle)
                {
                // Handle Circle button click
                    PdfViewer.AnnotationMode = AnnotationMode.Circle;
                }
            }
    }

    /// <summary>
    /// Event handler for the annotation menu button click.
    /// Manages the visibility of annotation-related UI elements and updates the PDF viewer's annotation mode.
    /// </summary>
    private void AnnotationMenu_Clicked(object sender, EventArgs e)
    {
        if(SelectedAnnotation!=null)
        {
            AnnotationIcons.IsVisible = true;
            AnnotationMenuGrid.IsVisible = true;
            Lock.IsVisible = false;
            Unlock.IsVisible = false;
            ColorPalette.IsVisible = true;
            Delete.IsVisible = false;           
        }
        else
        {
            AnnotationMenuGrid.IsVisible = !AnnotationMenuGrid.IsVisible;
        }
        if (!AnnotationMenuGrid.IsVisible)
        {
            PdfViewer.AnnotationMode = AnnotationMode.None;
            EditorControl.IsVisible = false;
            
        }
            
    }

    /// <summary>
    /// Handles the event when the color palette button is clicked, toggling the visibility of the editor control.
    /// </summary>
    private void ColorPalette_Clicked(object sender, EventArgs e)
    {
        EditorControl.IsVisible = !EditorControl.IsVisible;
          
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
        PdfViewer.SaveDocument(outputStream);
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
        PdfViewer.ExportAnnotations(xfdfStream, Syncfusion.Pdf.Parsing.AnnotationDataFormat.XFdf);
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
    private void EditOption_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible) && !EditOption.IsVisible)
        {
            EditorControl.IsVisible = false;
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
        Unlock.IsVisible = SelectedAnnotation.IsLocked;
        Lock.IsVisible = !SelectedAnnotation.IsLocked;
    }
    private void  Import_Clicked(object sender, EventArgs e)
    {
        string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "Export.xfdf");
        if (File.Exists(fileName))
        {
            Stream inputStream = File.OpenRead(fileName);
            inputStream.Position = 0;
            PdfViewer.ImportAnnotations(inputStream, Syncfusion.Pdf.Parsing.AnnotationDataFormat.XFdf);
            DisplayAlert("Information", "Annotations Loaded from the " + fileName, "OK");
        }
        else
            DisplayAlert("XFDF file Not Found", "No xfdf files available for import. Please export the annotations to an xfdf file and then import. ", "OK");
    }
}