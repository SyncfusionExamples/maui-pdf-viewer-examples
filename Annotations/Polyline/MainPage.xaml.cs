

using Syncfusion.Maui.PdfViewer;
using Syncfusion.Maui.Sliders;
using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;


using Syncfusion.Pdf.Parsing;
using System.ComponentModel;




namespace PolylineAnnotationDemo;

public partial class MainPage : ContentPage
{
    Stream exportedAnnotationStream = null;
    Annotation SelectedAnnotation;
    List<Color> colors;
    List<string> colorStrings;
    float opacity = 1f;
    int thickness = 5;
    public MainPage()
    {
        InitializeComponent();
        string[,] colorCodes = new string[1, 5] {
            { "#FF990000", "#FF996100", "#FF009907", "#FF060099", "#FF990098"},
        };
        colorStrings = new List<string>()
        {
            "---",
            "Red",
            "Green",
            "Blue",
            "Pink",
            "Cyan",
            "Yellow"
        };
        colorPicker.ItemsSource = colorStrings;
        colorPicker.SelectedIndex = 0;

        thickSlider.Value = 5;
        colors = new List<Color>()
        {
            null,
            Colors.Red,
            Colors.Green,
            Colors.Blue,
            Colors.Pink,
            Colors.Cyan,
            Colors.Yellow
        };
        // EditorControl.ConfigureColorPicker(colorCodes);
        // EditorControl.ConfigureOpacity(1);
        //EditorControl.ConfigureThickness();
        // EditorControl.ColorChanged += EditorControl_ColorChanged;
        // EditorControl.OpacityChangedEnd += EditorControl_OpacityChangedEnd;
        // EditorControl.ThicknessChangedEnd += EditorControl_ThicknessChangedEnd;
        PdfViewer.AnnotationSelected += PdfViewer_AnnotationSelected;
        PdfViewer.AnnotationDeselected += PdfViewer_AnnotationDeselected;
#if ANDROID
    Export.Margin = new Thickness(1,0,0,0);
#elif MACCATALYST
    Save.Margin = new Thickness(4,0,4,0);
#elif WINDOWS
        Save.Margin = new Thickness(4, 0, 9, 0);
#endif
        Stream? documentStream = this.GetType().Assembly.GetManifestResourceStream("PolylineAnnotationDemo.Assets." + "Annotations1.pdf");
        PdfViewer.LoadDocumentAsync(documentStream, flattenOptions: FlattenOptions.Unsupported);
       
    }

    /// <summary>
    /// Updates the opacity of the selected annotation or the default annotation settings based on the EditorControl's opacity change.
    /// </summary>
    /// 

   
   
    

   
    private void PdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = null;
        ColorGrid.IsVisible = false;
        OpacSlider.Value = 1;
        thickSlider.Value = 5;
        // ColorPalette.IsVisible = false;
        
        //EditorControl.IsVisible = false;
        AnnotationIcons.IsVisible=true;
        Delete.IsVisible = false;
        Unlock.IsVisible = false;
        Lock.IsVisible = false;
 
        //EditorControl.HeightRequest = 200f;

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
        ColorGrid.IsVisible = false;


        if (SelectedAnnotation is UnderlineAnnotation  || SelectedAnnotation is StrikeOutAnnotation  ||
                SelectedAnnotation is SquigglyAnnotation  ||
                SelectedAnnotation is HighlightAnnotation )
        {
            OpacSlider.Value = SelectedAnnotation.Opacity;
            
            //EditorControl.EditorOpacity=SelectedAnnotation.Opacity;
            //EditorControl.ThicknessSliderLayOut.IsVisible = false;
            //EditorControl.ColorPaletteLayOut.IsVisible = true;
            //EditorControl.OpacityThicknessSeperator.IsVisible = false;
            //EditorControl.ColorOpacitySeperator.IsVisible = true;
            //EditorControl.HeightRequest = 123f;
        }
        else if (SelectedAnnotation is StampAnnotation stamp)
        {
            OpacSlider.Value = SelectedAnnotation.Opacity;
            //EditorControl.EditorOpacity=stamp.Opacity;
            //EditorControl.ColorPaletteLayOut.IsVisible = false;
            //EditorControl.ThicknessSliderLayOut.IsVisible = false;
            //EditorControl.OpacityThicknessSeperator.IsVisible = false;
            //EditorControl.ColorOpacitySeperator.IsVisible = false;
            //EditorControl.HeightRequest = 90f;
        }
        else
        {
            //EditorControl.ThicknessSliderLayOut.IsVisible = true;
            //EditorControl.ColorPaletteLayOut.IsVisible = true;
            //EditorControl.ColorOpacitySeperator.IsVisible = true;
            //EditorControl.OpacityThicknessSeperator.IsVisible = true;
            
            //EditorControl.HeightRequest = 200f;
        }
        if (SelectedAnnotation is ShapeAnnotation shape)
        {
            thickSlider.Value= shape.BorderWidth;
            OpacSlider.Value = shape.Opacity;
            //EditorControl.EditorThickness = shape.BorderWidth;
            //EditorControl.EditorOpacity= shape.Opacity;

        }
        else if (SelectedAnnotation is InkAnnotation ink)
        {
            thickSlider.Value = ink.BorderWidth;
            OpacSlider.Value = ink.Opacity;
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
            else if (clickedButton == Polyline)
            {
                // Handle Circle button click
               // PdfViewer.AnnotationMode = AnnotationMode.Polyline;
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
           ColorGrid.IsVisible = false;
            
        }
            
    }

    /// <summary>
    /// Handles the event when the color palette button is clicked, toggling the visibility of the editor control.
    /// </summary>
    private void ColorPalette_Clicked(object sender, EventArgs e)
    {
        ColorGrid.IsVisible=!ColorGrid.IsVisible;
       // EditorControl.IsVisible = !EditorControl.IsVisible;
          
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
        MemoryStream stream = new MemoryStream();
        string filePath = null;

        try
        {
            PdfViewer.SaveDocument(stream);
            string fileName = "SavedPDF.pdf";

#if WINDOWS
    filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
    {
        stream.CopyTo(fileStream);
    }
#elif MACCATALYST || IOS
    filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
    {
        stream.CopyTo(fileStream);
    }
#elif ANDROID
    filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
    {
        stream.CopyTo(fileStream);
    }
    PdfViewer.LoadDocument(stream);
#endif

            DisplayAlert("Saved", filePath, "ok");
        }
        catch (Exception ex)
        {
            // Handle the exception or log the error message
            // For example, you can display an error message or log the exception details
            DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }


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
        Stream stream = new MemoryStream();
        string filePath = null;

        try
        {
            PdfViewer.ExportAnnotations(stream, AnnotationDataFormat.XFdf);
            string fileName = "ExportedAnntations.xfdf";

#if WINDOWS
    filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
    {
        stream.CopyTo(fileStream);
    }
#elif MACCATALYST || IOS
    filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
    {
        stream.CopyTo(fileStream);
    }
#elif ANDROID
    filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
    {
        stream.CopyTo(fileStream);
    }
    // PdfViewer.LoadDocument(stream);
#endif

            exportedAnnotationStream = stream;

            await Application.Current!.MainPage!.DisplayAlert("Annotations exported", $"The annotations are exported to the file {filePath}", "OK");
        }
        catch (Exception ex)
        {
            // Handle the exception or log the error message
            // For example, you can display an error message or log the exception details
            await Application.Current!.MainPage!.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }

    }
    string WriteFile(Stream stream, string fileName)
    {
        string filePath = string.Empty;
        if (stream != null)
        {
            stream.Position = 0;
#if MACCATALYST || IOS
                filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
#elif WINDOWS
filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
#else
            filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
#endif
            FileStream file = File.Create(filePath);
            stream.CopyTo(file);
            stream.Close();
            file.Close();
        }
        return filePath;
    }

    /// <summary>
    /// Copies the contents of the input stream to a file in the application's data directory.
    /// </summary>
    /// <param name="inputStream">The input stream containing the data to be copied.</param>
    /// <param name="filename">The name of the target file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task CopyFileToAppDataDirectory(Stream inputStream, string fileName)
    {
        //string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        //using FileStream outputStream = File.Create(targetFile);
        //await inputStream.CopyToAsync(outputStream);
        //await DisplayAlert("Annotations Exported", "The annotations are exported to the file " + targetFile  , " OK");
    }
    private void EditOption_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible) && !EditOption.IsVisible)
        {
           // EditorControl.IsVisible = false;
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
    private async void  Import_Clicked(object sender, EventArgs e)
    {
        Stream? stream=this.GetType().Assembly.GetManifestResourceStream("PolylineAnnotationDemo.Assets." + "ExportedAnnotations.xfdf");
        if (stream != null)
        {
            await PdfViewer.ImportAnnotationsAsync(stream, Syncfusion.Pdf.Parsing.AnnotationDataFormat.XFdf);
        }
        await Application.Current!.MainPage!.DisplayAlert("Annotations imported", $"The annotations are imported to this file", "OK");

    }

    private void TextSelected(object sender, TextSelectionChangedEventArgs e)
    {
      //  EditorControl.IsVisible = false;
    }




    // Enable or activate the polyline drawing mode.
    
    private void ValueChanged(object sender, ValueChangedEventArgs e)
    {

    }

    private void Color_Picked(object sender, EventArgs e)
    {
        Color color = Colors.Red;

        if(colorPicker.SelectedIndex!=0)
        {
            if (SelectedAnnotation != null)
            {
                color = colors[colorPicker.SelectedIndex];
                SelectedAnnotation.Color = color;
            }
            else 
            {
                color = colors[colorPicker.SelectedIndex];
                PdfViewer.AnnotationSettings.Square.Color = color;
                PdfViewer.AnnotationSettings.Circle.Color = color;
                PdfViewer.AnnotationSettings.Line.Color = color;
                PdfViewer.AnnotationSettings.Arrow.Color = color;
                PdfViewer.AnnotationSettings.Highlight.Color = color;
                PdfViewer.AnnotationSettings.Squiggly.Color = color;
                PdfViewer.AnnotationSettings.Underline.Color = color;
                PdfViewer.AnnotationSettings.StrikeOut.Color= color;
               // PdfViewer.AnnotationSettings.Polyline.Color = color;
                
            }
        }
       
    }

    private void OpacityValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (SelectedAnnotation != null)
        {
             SelectedAnnotation.Opacity = (float)e.NewValue;
            OpacSlider.Value = (float)e.NewValue;
        }
        else
        {
            PdfViewer.AnnotationSettings.Arrow.Opacity = (float)e.NewValue;
            PdfViewer.AnnotationSettings.Line.Opacity = (float)e.NewValue;
            PdfViewer.AnnotationSettings.Square.Opacity = (float)e.NewValue;
            PdfViewer.AnnotationSettings.Circle.Opacity = (float)e.NewValue;
            PdfViewer.AnnotationSettings.Ink.Opacity = (float)e.NewValue;
            //PdfViewer.AnnotationSettings.Polyline.Opacity = (float)e.NewValue;
            PdfViewer.AnnotationSettings.Highlight.Opacity = (float)e.NewValue;
            PdfViewer.AnnotationSettings.Squiggly.Opacity = (float)e.NewValue;
            PdfViewer.AnnotationSettings.StrikeOut.Opacity = (float)e.NewValue;
            PdfViewer.AnnotationSettings.Underline.Opacity = (float)e.NewValue;
            OpacSlider.Value = (float)e.NewValue;
        }
    }

    private void ThicknessValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            if (SelectedAnnotation is ShapeAnnotation shape)
            {
                shape.BorderWidth = (int)e.NewValue;
                thickSlider.Value = (int)e.NewValue;
            }
            if (SelectedAnnotation is InkAnnotation ink)
            {
                ink.BorderWidth = (int)e.NewValue;
                thickSlider.Value = (int)e.NewValue;
            }

        }
        else
        {
            PdfViewer.AnnotationSettings.Arrow.BorderWidth = (int)e.NewValue;
            PdfViewer.AnnotationSettings.Line.BorderWidth = (int)e.NewValue;
            PdfViewer.AnnotationSettings.Square.BorderWidth = (int)e.NewValue;
            PdfViewer.AnnotationSettings.Circle.BorderWidth = (int)e.NewValue;
            PdfViewer.AnnotationSettings.Ink.BorderWidth = (int)e.NewValue;
            thickSlider.Value = (int)e.NewValue;
            // PdfViewer.AnnotationSettings.Polyline.BorderWidth = (int)e.NewValue;
        }
    }

    private void UndoRedoClicked(object sender, EventArgs e)
    {
        ColorGrid.IsVisible = false;
    }

    private async void ImportExportedFile_Clicked(object sender, EventArgs e)
    {
        //Stream stream = new MemoryStream();
        //stream = exportedAnnotationStream;
        //if (stream != null)
        //{
           
        //        await PdfViewer.ImportAnnotationsAsync(stream, Syncfusion.Pdf.Parsing.AnnotationDataFormat.XFdf);
            
        //    await Application.Current!.MainPage!.DisplayAlert("Annotations imported", $"The annotations are imported to this file", "OK");

        //}
        //else
        //    await Application.Current!.MainPage!.DisplayAlert(" Export Action not taken yet", "Please Export and then proceed Import", "OK");
    }
}