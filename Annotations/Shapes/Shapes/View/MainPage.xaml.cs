using Microsoft.Maui.Platform;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Maui.Sliders;
using System.Windows.Input;

namespace Shapes;

public partial class MainPage : ContentPage
{
   
    Annotation? SelectedAnnotation;
    public MainPage()
    {
        InitializeComponent();
        string[,] colorCodes = new string[1, 5] {
            { "#FF990000", "#FF996100", "#FF009907", "#FF060099", "#FF990098"},
        };
        shapeColorPaletteEditor.SelectedThickness = PdfViewer.AnnotationSettings.Square.BorderWidth;
        shapeColorPaletteEditor.SelectedOpacity=PdfViewer.AnnotationSettings.Circle.Color.Alpha;
        shapeColorPaletteEditor.SelectedFillColorOpacity = 1;
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedThickness = shapeColorPaletteEditor.SelectedThickness;
            viewModel.SelectedOpacity = shapeColorPaletteEditor.SelectedOpacity;
            viewModel.SelectedFillColorOpacity = shapeColorPaletteEditor.SelectedFillColorOpacity;
        }
        shapeColorPaletteEditor.ShapeStrokeColorChanged += ShapeColorPaletteEditor_ShapeStrokeColorChanged;
        shapeColorPaletteEditor.ShapeFillColorChanged += ShapeColorPaletteEditor_ShapeFillColorChanged;
        shapeColorPaletteEditor.StrokeOpacityChanged += ShapeColorPaletteEditor_StrokeOpacityChanged;
        shapeColorPaletteEditor.FillOpacityChanged += ShapeColorPaletteEditor_FillOpacityChanged;
        shapeColorPaletteEditor.BorderThicknessChanged += ShapeColorPaletteEditor_BorderThicknessChanged;
        PdfViewer.AnnotationSelected += PdfViewer_AnnotationSelected;
        PdfViewer.AnnotationDeselected += PdfViewer_AnnotationDeselected; ;
#if ANDROID
    Export.Margin = new Thickness(1,0,0,0);
#elif MACCATALYST
    Save.Margin = new Thickness(4,0,4,0);
#elif WINDOWS
        Save.Margin = new Thickness(4, 0, 9, 0);
#endif
    }

    private void ShapeColorPaletteEditor_BorderThicknessChanged(object? sender, float e)
    {
        if (SelectedAnnotation != null)
        {
            if (SelectedAnnotation is SquareAnnotation squareAnnotation)
                squareAnnotation.BorderWidth = e;
            else if (SelectedAnnotation is LineAnnotation lineAnnotation)
                lineAnnotation.BorderWidth = e;
            else if (SelectedAnnotation is PolygonAnnotation polygonAnnotation)
                polygonAnnotation.BorderWidth = e;
            else if (SelectedAnnotation is PolylineAnnotation polylineAnnotation)
                polylineAnnotation.BorderWidth = e;
            else if (SelectedAnnotation is CircleAnnotation circleAnnotation)
                circleAnnotation.BorderWidth = e;
            else if (SelectedAnnotation is InkAnnotation inkAnnotation)
                inkAnnotation.BorderWidth = e;
        }
        else
        {
            PdfViewer.AnnotationSettings.Square.BorderWidth = e;
            PdfViewer.AnnotationSettings.Circle.BorderWidth = e;
            PdfViewer.AnnotationSettings.Arrow.BorderWidth = e;
            PdfViewer.AnnotationSettings.Line.BorderWidth = e;
            PdfViewer.AnnotationSettings.Polygon.BorderWidth = e;
            PdfViewer.AnnotationSettings.Polyline.BorderWidth = e;
        }
    }

    private void ShapeColorPaletteEditor_FillOpacityChanged(object? sender, float e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.FillColor = SelectedAnnotation.FillColor.WithAlpha(e);
        }
        else if (PdfViewer.AnnotationMode != AnnotationMode.None && PdfViewer.AnnotationMode != AnnotationMode.Polyline && PdfViewer.AnnotationMode != AnnotationMode.Line && PdfViewer.AnnotationMode != AnnotationMode.Arrow)
        {
            switch (PdfViewer.AnnotationMode)
            {
                case AnnotationMode.Square:
                    PdfViewer.AnnotationSettings.Square.FillColor = PdfViewer.AnnotationSettings.Square.Color.WithAlpha(e);
                    break;
                case AnnotationMode.Circle:
                    PdfViewer.AnnotationSettings.Circle.FillColor = PdfViewer.AnnotationSettings.Circle.FillColor.WithAlpha(e);
                    break;
                case AnnotationMode.Polygon:
                    PdfViewer.AnnotationSettings.Polygon.FillColor = PdfViewer.AnnotationSettings.Polygon.FillColor.WithAlpha(e);
                    break;
            }
        }
        else
        {
            PdfViewer.AnnotationSettings.Square.FillColor = PdfViewer.AnnotationSettings.Square.Color.WithAlpha(e);
            PdfViewer.AnnotationSettings.Circle.FillColor = PdfViewer.AnnotationSettings.Circle.FillColor.WithAlpha(e);
            PdfViewer.AnnotationSettings.Polygon.FillColor = PdfViewer.AnnotationSettings.Polygon.FillColor.WithAlpha(e);
        }
    }

    private void ShapeColorPaletteEditor_StrokeOpacityChanged(object? sender, float e)
    {
        if (SelectedAnnotation != null)
        {
            if (SelectedAnnotation is StampAnnotation)
                SelectedAnnotation.Opacity = e;
            else
                SelectedAnnotation.Color = SelectedAnnotation.Color.WithAlpha(e);
        }
        else if (PdfViewer.AnnotationMode != AnnotationMode.None)
        {
            switch (PdfViewer.AnnotationMode)
            {
                case AnnotationMode.Square:
                    PdfViewer.AnnotationSettings.Square.Color = PdfViewer.AnnotationSettings.Square.Color.WithAlpha(e);
                    break;
                case AnnotationMode.Circle:
                    PdfViewer.AnnotationSettings.Circle.Color = PdfViewer.AnnotationSettings.Circle.Color.WithAlpha(e);
                    break;
                case AnnotationMode.Arrow:
                    PdfViewer.AnnotationSettings.Arrow.Color = PdfViewer.AnnotationSettings.Arrow.Color.WithAlpha(e);
                    break;
                case AnnotationMode.Polygon:
                    PdfViewer.AnnotationSettings.Polygon.Color = PdfViewer.AnnotationSettings.Polygon.Color.WithAlpha(e);
                    break;
                case AnnotationMode.Line:
                    PdfViewer.AnnotationSettings.Line.Color = PdfViewer.AnnotationSettings.Line.Color.WithAlpha(e);
                    break;
                case AnnotationMode.Polyline:
                    PdfViewer.AnnotationSettings.Polyline.Color = PdfViewer.AnnotationSettings.Polyline.Color.WithAlpha(e);
                    break;
            }
        }
        else
        {
            PdfViewer.AnnotationSettings.Square.Color = PdfViewer.AnnotationSettings.Square.Color.WithAlpha(e);
            PdfViewer.AnnotationSettings.Circle.Color = PdfViewer.AnnotationSettings.Circle.Color.WithAlpha(e);
            PdfViewer.AnnotationSettings.Line.Color = PdfViewer.AnnotationSettings.Line.Color.WithAlpha(e);
            PdfViewer.AnnotationSettings.Arrow.Color = PdfViewer.AnnotationSettings.Arrow.Color.WithAlpha(e);
            PdfViewer.AnnotationSettings.Polygon.Color = PdfViewer.AnnotationSettings.Polygon.Color.WithAlpha(e);
            PdfViewer.AnnotationSettings.Polyline.Color = PdfViewer.AnnotationSettings.Polyline.Color.WithAlpha(e);
        }
    }

    private void ShapeColorPaletteEditor_ShapeFillColorChanged(object? sender, Color e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.FillColor = e.WithAlpha(shapeColorPaletteEditor.SelectedFillColorOpacity);
        }
        else if (PdfViewer.AnnotationMode != AnnotationMode.None && PdfViewer.AnnotationMode != AnnotationMode.Polyline && PdfViewer.AnnotationMode != AnnotationMode.Line && PdfViewer.AnnotationMode != AnnotationMode.Arrow)
        {
            switch (PdfViewer.AnnotationMode)
            {
                case AnnotationMode.Square:
                    PdfViewer.AnnotationSettings.Square.FillColor = e.WithAlpha(shapeColorPaletteEditor.SelectedFillColorOpacity);
                    break;
                case AnnotationMode.Circle:
                    PdfViewer.AnnotationSettings.Circle.FillColor = e.WithAlpha(shapeColorPaletteEditor.SelectedFillColorOpacity);
                    break;
                case AnnotationMode.Polygon:
                    PdfViewer.AnnotationSettings.Polygon.FillColor = e.WithAlpha(shapeColorPaletteEditor.SelectedFillColorOpacity);
                    break;
            }
        }
        else
        {
            PdfViewer.AnnotationSettings.Square.FillColor = e.WithAlpha(shapeColorPaletteEditor.SelectedFillColorOpacity);
            PdfViewer.AnnotationSettings.Circle.FillColor = e.WithAlpha(shapeColorPaletteEditor.SelectedFillColorOpacity);
            PdfViewer.AnnotationSettings.Polygon.FillColor = e.WithAlpha(shapeColorPaletteEditor.SelectedFillColorOpacity);
        }
    }

    private void ShapeColorPaletteEditor_ShapeStrokeColorChanged(object? sender, Color e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
        }
        else if (PdfViewer.AnnotationMode != AnnotationMode.None)
        {
            switch (PdfViewer.AnnotationMode)
            {
                case AnnotationMode.Square:
                    PdfViewer.AnnotationSettings.Square.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
                    break;
                case AnnotationMode.Circle:
                    PdfViewer.AnnotationSettings.Circle.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
                    break;
                case AnnotationMode.Arrow:
                    PdfViewer.AnnotationSettings.Arrow.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
                    break;
                case AnnotationMode.Polygon:
                    PdfViewer.AnnotationSettings.Polygon.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
                    break;
                case AnnotationMode.Line:
                    PdfViewer.AnnotationSettings.Line.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
                    break;
                case AnnotationMode.Polyline:
                    PdfViewer.AnnotationSettings.Polyline.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
                    break;
            }
        }
        else
        {
            PdfViewer.AnnotationSettings.Square.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
            PdfViewer.AnnotationSettings.Circle.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
            PdfViewer.AnnotationSettings.Arrow.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
            PdfViewer.AnnotationSettings.Line.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
            PdfViewer.AnnotationSettings.Polygon.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
            PdfViewer.AnnotationSettings.Polyline.Color = e.WithAlpha(shapeColorPaletteEditor.SelectedOpacity);
        }
    }

    private void PdfViewer_AnnotationDeselected(object? sender, AnnotationEventArgs e)
    { 
            SelectedAnnotation = null;
        // ColorPalatte.IsVisible = false;
        shapeColorPaletteEditor.IsVisible = false;
           shapeList.IsVisible = true;
           delete.IsVisible = false;
           Lock.IsVisible = false;
           Unlock.IsVisible = false;
           colorPalette.IsEnabled = true;
    }

    private void PdfViewer_AnnotationSelected(object? sender, AnnotationEventArgs e)
    {
        if (e.Annotation != null)
            SelectedAnnotation = e.Annotation;
        shapeList.IsVisible = false;
        AnnotationMenuGrid.IsVisible = true;
        delete.IsVisible = true;
        shapeColorPaletteEditor.IsVisible = false;
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            if (SelectedAnnotation is SquareAnnotation || SelectedAnnotation is CircleAnnotation || SelectedAnnotation is PolygonAnnotation)
                viewModel.IsSelectedAnnotationClosed = true;
            else
                viewModel.IsSelectedAnnotationClosed = false;
        }
        SetSliderValue();
        if(SelectedAnnotation!=null)
        {
            if (SelectedAnnotation.IsLocked)
            {
                Unlock.IsVisible = true;
                Lock.IsVisible = false;
                colorPalette.IsEnabled = false;
                delete.IsEnabled = false;
            }
            else
            {
                Unlock.IsVisible = false;
                colorPalette.IsEnabled = true;
                delete.IsEnabled = true;
                Lock.IsVisible = true;
            }
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
            if (clickedButton == Rectangle)
            {
                PdfViewer.AnnotationMode = AnnotationMode.Square;
            }
            else if (clickedButton == Circle)
            {
                PdfViewer.AnnotationMode= AnnotationMode.Circle;
            }
            else if (clickedButton == Arrow)
            {
                PdfViewer.AnnotationMode = AnnotationMode.Arrow;
            }
            else if (clickedButton == Line)
            {
                PdfViewer.AnnotationMode = AnnotationMode.Line;
            }
            else if (clickedButton == Polygon)
            {
                PdfViewer.AnnotationMode=AnnotationMode.Polygon;
            }
            else if (clickedButton == Polyline)
            {
                PdfViewer.AnnotationMode = AnnotationMode.Polyline;
            }
            SetSliderValue();
            shapeColorPaletteEditor.IsVisible = false;
            // Add more cases if needed
        }
    }

    internal void SetSliderValue()
    {
        if(SelectedAnnotation != null)
        {
            shapeColorPaletteEditor.SelectedFillColorOpacity = 1;
            if (SelectedAnnotation is SquareAnnotation square)
            {
                shapeColorPaletteEditor.SelectedThickness = square.BorderWidth;
                shapeColorPaletteEditor.SelectedOpacity = square.Color.Alpha;
                shapeColorPaletteEditor.SelectedFillColorOpacity = square.FillColor.Alpha;
            }
            else if(SelectedAnnotation is CircleAnnotation circle)
            {
                shapeColorPaletteEditor.SelectedThickness = circle.BorderWidth;
                shapeColorPaletteEditor.SelectedOpacity = circle.Color.Alpha;
                shapeColorPaletteEditor.SelectedFillColorOpacity = circle.FillColor.Alpha;
            }
            else if(SelectedAnnotation is LineAnnotation line)
            {
                shapeColorPaletteEditor.SelectedThickness = line.BorderWidth;
                shapeColorPaletteEditor.SelectedOpacity = line.Color.Alpha;
            }
            else if(SelectedAnnotation is PolygonAnnotation polygon)
            {
                shapeColorPaletteEditor.SelectedThickness =polygon.BorderWidth;
                shapeColorPaletteEditor.SelectedOpacity = polygon.Color.Alpha;
                shapeColorPaletteEditor.SelectedFillColorOpacity = polygon.FillColor.Alpha;
            }
            else if(SelectedAnnotation is PolylineAnnotation polyline)
            {
                shapeColorPaletteEditor.SelectedThickness = polyline.BorderWidth;
                shapeColorPaletteEditor.SelectedOpacity = polyline.Color.Alpha;
            }
            else if(SelectedAnnotation is InkAnnotation ink)
            {
                shapeColorPaletteEditor.SelectedThickness = ink.BorderWidth;
                shapeColorPaletteEditor.SelectedOpacity = ink.Color.Alpha;
            }
            else if(SelectedAnnotation is StampAnnotation stamp)
            {
                shapeColorPaletteEditor.SelectedOpacity = stamp.Opacity;
            }
        }
        else
        {
            switch(PdfViewer.AnnotationMode)
            {
                case AnnotationMode.Square:
                    shapeColorPaletteEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Square.Color.Alpha;
                    shapeColorPaletteEditor.SelectedThickness = PdfViewer.AnnotationSettings.Square.BorderWidth;
                    break;
                case AnnotationMode.Circle:
                    shapeColorPaletteEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Circle.Color.Alpha;
                    shapeColorPaletteEditor.SelectedThickness = PdfViewer.AnnotationSettings.Circle.BorderWidth;
                    break;
                case AnnotationMode.Arrow:
                    shapeColorPaletteEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Arrow.Color.Alpha;
                    shapeColorPaletteEditor.SelectedThickness = PdfViewer.AnnotationSettings.Arrow.BorderWidth;
                    break;
                case AnnotationMode.Polygon:
                    shapeColorPaletteEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Polygon.Color.Alpha;
                    shapeColorPaletteEditor.SelectedThickness = PdfViewer.AnnotationSettings.Polygon.BorderWidth;
                    break;
                case AnnotationMode.Line:
                    shapeColorPaletteEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Line.Color.Alpha;
                    shapeColorPaletteEditor.SelectedThickness = PdfViewer.AnnotationSettings.Line.BorderWidth;
                    break;
                case AnnotationMode.Polyline:
                    shapeColorPaletteEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Polyline.Color.Alpha;
                    shapeColorPaletteEditor.SelectedThickness = PdfViewer.AnnotationSettings.Line.BorderWidth;
                    break;
            }
        }
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedThickness = shapeColorPaletteEditor.SelectedThickness;
            viewModel.SelectedOpacity = shapeColorPaletteEditor.SelectedOpacity;
            viewModel.SelectedFillColorOpacity=shapeColorPaletteEditor.SelectedFillColorOpacity;
            if (SelectedAnnotation is SquareAnnotation || SelectedAnnotation is CircleAnnotation || SelectedAnnotation is PolygonAnnotation || PdfViewer.AnnotationMode==AnnotationMode.Polygon || PdfViewer.AnnotationMode==AnnotationMode.Circle|| PdfViewer.AnnotationMode==AnnotationMode.Square)
                viewModel.IsSelectedAnnotationClosed = true;
            else
                viewModel.IsSelectedAnnotationClosed = false;
        }
    }

    /// <summary>
    /// Handles the event when the color palette button is clicked, toggling the visibility of the editor control.
    /// </summary>
    private void ColorPalette_Clicked(object sender, EventArgs e)
    {
        shapeColorPaletteEditor.IsVisible = !shapeColorPaletteEditor.IsVisible;
        //EditorControl.IsVisible = !EditorControl.IsVisible;

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
            //EditorControl.IsVisible = false;
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
            colorPalette.IsEnabled = !SelectedAnnotation.IsLocked;
            delete.IsEnabled = !SelectedAnnotation.IsLocked;
        }
        if(SelectedAnnotation != null)
        {
            Unlock.IsVisible = SelectedAnnotation.IsLocked;
            Lock.IsVisible = !SelectedAnnotation.IsLocked;
        }
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
        shapeColorPaletteEditor.IsVisible = !shapeColorPaletteEditor.IsVisible;
    }

    private void ShapesButtonClicked(object sender, EventArgs e)
    {
        if(AnnotationMenuGrid.IsVisible && !shapeList.IsVisible)
        {
            if(SelectedAnnotation!=null)
            {
                PdfViewer.DeselectAnnotation(SelectedAnnotation);
                shapeList.IsVisible = true;
            }
        }
        else
        {
            if (AnnotationMenuGrid.IsVisible)
            {
                PdfViewer.AnnotationMode = AnnotationMode.None;
                shapeColorPaletteEditor.IsVisible = false;
            }
            AnnotationMenuGrid.IsVisible = !AnnotationMenuGrid.IsVisible;
        }
        SetSliderValue();
        
    }
}