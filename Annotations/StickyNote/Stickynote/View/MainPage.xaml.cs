using Microsoft.Maui.Platform;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Maui.Sliders;
using System.Windows.Input;

namespace Stickynote;

public partial class MainPage : ContentPage
{

    Annotation? SelectedAnnotation;
    public MainPage()
    {
        InitializeComponent();
        PdfViewer.ShowToolbars = false;
        string[,] colorCodes = new string[1, 5] {
            { "#FF990000", "#FF996100", "#FF009907", "#FF060099", "#FF990098"},
        };
        stickyColorPaletteEditor.SelectedOpacity = PdfViewer.AnnotationSettings.StickyNote.Color.Alpha;
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedOpacity = stickyColorPaletteEditor.SelectedOpacity;
        }
        stickyColorPaletteEditor.StickyStrokeColorChanged += StickyColorPaletteEditor_StickyStrokeColorChanged;
        stickyColorPaletteEditor.StickyFillColorChanged += StickyColorPaletteEditor_StickyFillColorChanged;
        stickyColorPaletteEditor.StrokeOpacityChanged += StickyColorPaletteEditor_StrokeOpacityChanged;
        PdfViewer.AnnotationSelected += PdfViewer_AnnotationSelected;
        PdfViewer.AnnotationDeselected += PdfViewer_AnnotationDeselected;
        PdfViewer.Tapped += PdfViewer_Tapped;
#if ANDROID
        Export.Margin = new Thickness(1, 0, 0, 0);
#elif MACCATALYST
    Save.Margin = new Thickness(4,0,4,0);
#elif WINDOWS
        Save.Margin = new Thickness(4, 0, 9, 0);
#endif
    }

    private void PdfViewer_Tapped(object? sender, GestureEventArgs? e)
    {
        stickyColorPaletteEditor.IsVisible = false;
    }

    private void StickyColorPaletteEditor_StrokeOpacityChanged(object? sender, float e)
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
            PdfViewer.AnnotationSettings.StickyNote.Color = PdfViewer.AnnotationSettings.StickyNote.Color.WithAlpha(e);
        }
        else
        {
            PdfViewer.AnnotationSettings.StickyNote.Color = PdfViewer.AnnotationSettings.StickyNote.Color.WithAlpha(e);
        }
    }

    private void StickyColorPaletteEditor_StickyFillColorChanged(object? sender, Color e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.FillColor = e.WithAlpha(stickyColorPaletteEditor.SelectedFillColorOpacity);
        }
        else if (PdfViewer.AnnotationMode != AnnotationMode.None && PdfViewer.AnnotationMode != AnnotationMode.Polyline && PdfViewer.AnnotationMode != AnnotationMode.Line && PdfViewer.AnnotationMode != AnnotationMode.Arrow)
        {
            switch (PdfViewer.AnnotationMode)
            {
                case AnnotationMode.Square:
                    PdfViewer.AnnotationSettings.Square.FillColor = e.WithAlpha(stickyColorPaletteEditor.SelectedFillColorOpacity);
                    break;
                case AnnotationMode.Circle:
                    PdfViewer.AnnotationSettings.Circle.FillColor = e.WithAlpha(stickyColorPaletteEditor.SelectedFillColorOpacity);
                    break;
                case AnnotationMode.Polygon:
                    PdfViewer.AnnotationSettings.Polygon.FillColor = e.WithAlpha(stickyColorPaletteEditor.SelectedFillColorOpacity);
                    break;
            }
        }
        else
        {
            PdfViewer.AnnotationSettings.Square.FillColor = e.WithAlpha(stickyColorPaletteEditor.SelectedFillColorOpacity);
            PdfViewer.AnnotationSettings.Circle.FillColor = e.WithAlpha(stickyColorPaletteEditor.SelectedFillColorOpacity);
            PdfViewer.AnnotationSettings.Polygon.FillColor = e.WithAlpha(stickyColorPaletteEditor.SelectedFillColorOpacity);
        }
    }

    private void StickyColorPaletteEditor_StickyStrokeColorChanged(object? sender, Color e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Color = e.WithAlpha(stickyColorPaletteEditor.SelectedOpacity);
        }
        else if (PdfViewer.AnnotationMode != AnnotationMode.None)
        {
            PdfViewer.AnnotationSettings.StickyNote.Color = e.WithAlpha(stickyColorPaletteEditor.SelectedOpacity);

        }
        else
        {
            PdfViewer.AnnotationSettings.StickyNote.Color = e.WithAlpha(stickyColorPaletteEditor.SelectedOpacity);
        }
    }

    private void PdfViewer_AnnotationDeselected(object? sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = null;
        // ColorPalatte.IsVisible = false;
        stickyColorPaletteEditor.IsVisible = false;
        stickyList.IsVisible = true;
        delete.IsVisible = false;
        Lock.IsVisible = false;
        Unlock.IsVisible = false;
        colorPalette.IsEnabled = true;
    }

    private void PdfViewer_AnnotationSelected(object? sender, AnnotationEventArgs e)
    {
        if (e.Annotation != null)
            SelectedAnnotation = e.Annotation;
        stickyList.IsVisible = false;
        AnnotationMenuGrid.IsVisible = true;
        delete.IsVisible = true;
        stickyColorPaletteEditor.IsVisible = false;
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
                viewModel.IsSelectedAnnotationClosed = false;
        }
        SetSliderValue();
        if (SelectedAnnotation != null)
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
        PdfViewer.AnnotationMode=AnnotationMode.StickyNote;
        if (PdfViewer.AnnotationSettings.StickyNote is StickyNoteAnnotationSettings sticky)
        {
            if (sender is Button clickedButton)
            {
                if (clickedButton == Note)
                {
                    sticky.Icon = StickyNoteIcon.Note;
                }
                else if (clickedButton == Insert)
                {
                    sticky.Icon = StickyNoteIcon.Insert;
                }
                else if (clickedButton == Comment)
                {
                    sticky.Icon = StickyNoteIcon.Comment;
                }
                else if (clickedButton == Key)
                {
                    sticky.Icon = StickyNoteIcon.Key;
                }
                else if (clickedButton == Help)
                {
                    sticky.Icon = StickyNoteIcon.Help;
                }
                else if (clickedButton == Paragraph)
                {
                    sticky.Icon = StickyNoteIcon.Paragraph;
                }
                else if (clickedButton == NewParagraph)
                {
                    sticky.Icon = StickyNoteIcon.NewParagraph;
                }
                SetSliderValue();
                    stickyColorPaletteEditor.IsVisible = false;
                    // Add more cases if needed
            }
        }
    }

    internal void SetSliderValue()
    {
        if (SelectedAnnotation != null)
        {
            stickyColorPaletteEditor.SelectedFillColorOpacity = 1;
            if (SelectedAnnotation is StickyNoteAnnotation sticky)
            {
                stickyColorPaletteEditor.SelectedOpacity = sticky.Color.Alpha;
                stickyColorPaletteEditor.SelectedFillColorOpacity = sticky.FillColor.Alpha;
            }
            else if (SelectedAnnotation is StampAnnotation stamp)
            {
                stickyColorPaletteEditor.SelectedOpacity = stamp.Opacity;
            }
        }
        else
        {
            stickyColorPaletteEditor.SelectedOpacity = PdfViewer.AnnotationSettings.StickyNote.Color.Alpha;
        }
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedThickness = stickyColorPaletteEditor.SelectedThickness;
            viewModel.SelectedOpacity = stickyColorPaletteEditor.SelectedOpacity;
            viewModel.SelectedFillColorOpacity = stickyColorPaletteEditor.SelectedFillColorOpacity;
            if (SelectedAnnotation is SquareAnnotation || SelectedAnnotation is CircleAnnotation || SelectedAnnotation is PolygonAnnotation || PdfViewer.AnnotationMode == AnnotationMode.Polygon || PdfViewer.AnnotationMode == AnnotationMode.Circle || PdfViewer.AnnotationMode == AnnotationMode.Square)
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
        stickyColorPaletteEditor.IsVisible = !stickyColorPaletteEditor.IsVisible;
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
        await DisplayAlert("Annotations Exported", "The annotations are exported to the file " + targetFile, " OK");
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
        if (SelectedAnnotation != null)
        {
            Unlock.IsVisible = SelectedAnnotation.IsLocked;
            Lock.IsVisible = !SelectedAnnotation.IsLocked;
        }
    }
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

    private void ColorPalatte_Clicked(object sender, EventArgs e)
    {
        stickyColorPaletteEditor.IsVisible = !stickyColorPaletteEditor.IsVisible;
    }

    private void StickyButtonClicked(object sender, EventArgs e)
    {
        if (AnnotationMenuGrid.IsVisible && !stickyList.IsVisible)
        {
            if (SelectedAnnotation != null)
            {
                PdfViewer.DeselectAnnotation(SelectedAnnotation);
                stickyList.IsVisible = true;
            }
        }
        else
        {
            if (AnnotationMenuGrid.IsVisible)
            {
                PdfViewer.AnnotationMode = AnnotationMode.None;
                stickyColorPaletteEditor.IsVisible = false;
            }
            AnnotationMenuGrid.IsVisible = !AnnotationMenuGrid.IsVisible;
        }
        SetSliderValue();

    }
}