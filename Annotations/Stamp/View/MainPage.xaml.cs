using Microsoft.Maui.Platform;
using Syncfusion.Maui.PdfViewer;
using System.Reflection;
using Syncfusion.Maui.Sliders;

namespace Stamp;

public partial class MainPage : ContentPage
{

    Annotation SelectedAnnotation;

    /// <summary>
    /// Stores the type of the built-in stamp which is tapped in the stamp dialog.
    /// </summary>
    private StampType StampType { get; set; }

    /// <summary>
    /// Stores the image stream of the custom stamp which is tapped in the stamp dialog.
    /// </summary>
    private Stream ImageStream { get; set; }

    /// <summary>
    /// Indicates whether the tap on the PdfViewer should add the stamp selected from the stamp dialog. Once the stamp is added, this field will be set to false.
    /// </summary>
    private bool StampMode { get; set; } = false;

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
        Save.Margin = new Thickness(4, 0, 4, 0);
#elif WINDOWS
    Save.Margin = new Thickness(4,0,9,0);
#endif
        PdfViewer.Tapped += PdfViewer_Tapped;

        MessagingCenter.Subscribe<StampDialog, StampType>(this, "Built-inStamp", (sender, stampType) =>
        {
            StampType = stampType;
            ImageStream = null;
            StampMode = true;
        });

        MessagingCenter.Subscribe<StampDialog, Stream>(this, "CustomStamp", (sender, stream) =>
        {
            ImageStream = stream;
            StampMode = true;
        });
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

    #region Stamp Annotation
    /// <summary>
    /// Event handler for the tap event on the PdfViewer control. Adds a stamp annotation 
    /// to the PDF document based on the tap position.
    /// </summary>
    private void PdfViewer_Tapped(object sender, GestureEventArgs e)
    {
        if (StampMode)
        {
            PointF point = new PointF(e.PagePosition.X, e.PagePosition.Y);
            if (ImageStream == null)
            {
                StampAnnotation builtStamp = new StampAnnotation(StampType, e.PageNumber, point);
                PdfViewer.AddAnnotation(builtStamp);
            }
            else
            {
                StampAnnotation customStamp = new StampAnnotation(ImageStream, e.PageNumber, new RectF(point.X, point.Y, 79, 75));
                PdfViewer.AddAnnotation(customStamp);
            }
            StampMode = false;
        }
    }

    /// <summary>
    /// Handles the event when the thickness of the editor control changes and updates the thickness of the selected shape or ink annotation.
    /// </summary>
    private void EditorControl_ThicknessChangedEnd(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            if (SelectedAnnotation is ShapeAnnotation shape)
            {
                shape.BorderWidth = (int)EditorControl.EditorThickness;
            }
            else if (SelectedAnnotation is InkAnnotation ink)
            {
                ink.BorderWidth = (int)EditorControl.EditorThickness;
            }
        }
    }

    /// <summary>
    /// Handles the event when the color of the editor control changes and updates the color of the selected annotation.
    /// </summary>
    private void EditorControl_ColorChanged(object sender, ColorChangedEventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Color = Color.FromArgb(e.ColorCode);
        }
    }

    /// <summary>
    /// Handles the event when the opacity of the editor control changes and updates the opacity of the selected annotation.
    /// </summary>
    private void EditorControl_OpacityChangedEnd(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Opacity = (float)EditorControl.EditorOpacity;
        }
    }

    /// <summary>
    /// Handles the event when an annotation is deselected in the PdfViewer.
    /// </summary>
    private void PdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = null;
        AnnotationPropertyGrid.IsVisible = false;
        EditorControl.IsVisible = false;
        ResetEditorControlAppearance();
    }

    /// <summary>
    /// Handles the event when an annotation is selected in the PdfViewer.
    /// </summary>
    private void PdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = e.Annotation;
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
            EditorControl.OpacityThicknessSeparator.IsVisible = false;
            EditorControl.ColorOpacitySeparator.IsVisible = true;
            EditorControl.HeightRequest = 123f;
        }
        else if (SelectedAnnotation is StampAnnotation)
        {
            EditorControl.ColorPaletteLayOut.IsVisible = false;
            EditorControl.ThicknessSliderLayOut.IsVisible = false;
            EditorControl.OpacityThicknessSeparator.IsVisible = false;
            EditorControl.ColorOpacitySeparator.IsVisible = false;
            EditorControl.HeightRequest = 90f;
        }
        else
        {
            ResetEditorControlAppearance();
        }
        if (SelectedAnnotation is ShapeAnnotation shape)
        {
            EditorControl.EditorThickness = shape.BorderWidth;
        }
        else if (SelectedAnnotation is InkAnnotation ink)
        {
            EditorControl.EditorThickness = ink.BorderWidth;
        }
        EditorControl.EditorOpacity = SelectedAnnotation.Opacity;
    }

    /// <summary>
    /// Resets the appearance of the editor control to its default state.
    /// </summary>
    private void ResetEditorControlAppearance()
    {
        EditorControl.ThicknessSliderLayOut.IsVisible = true;
        EditorControl.ColorPaletteLayOut.IsVisible = true;
        EditorControl.OpacityThicknessSeparator.IsVisible = true;
        EditorControl.ColorOpacitySeparator.IsVisible = true;
        EditorControl.HeightRequest = 200f;
    }

    /// <summary>
    /// Event handler for the "PropertyChanged" event in the EditOption class. 
    /// Controls the visibility of the EditorControl based on changes to the "IsVisible" property in the EditOption class.
    /// </summary>
    private void EditOption_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible))
        {
            EditorControl.IsVisible = false;
        }
    }

    /// <summary>
    /// Handles the event when the color palette button is clicked, toggling the visibility of the editor control.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    private void ColorPalette_Clicked(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            EditorControl.IsVisible = !EditorControl.IsVisible;
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
    /// Handles the event when the "Show Stamp Dialog" button is clicked, toggling the visibility of stamp controls and editor.
    /// </summary>
    private void ShowStampDialog_Clicked(object sender, EventArgs e)
    {
        StampDialog.Dispatcher.Dispatch(() => StampDialog.IsVisible = true);
        AnnotationPropertyGrid.IsVisible = false;
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
        // Copy the file to the Directory
        using FileStream outputStream = File.Create(targetFile);
        await inputStream.CopyToAsync(outputStream);
        await DisplayAlert("Annotations exported", "The annotations are exported to the file " + targetFile, "OK");
    }
    #endregion
}