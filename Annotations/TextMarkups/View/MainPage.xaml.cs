using Microsoft.Maui.Platform;
using Microsoft.Maui.Storage;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Maui.Sliders;

namespace TextMarkups;

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
        EditorControl.ConfigureThickness(5);
        EditorControl.ColorChanged += EditorControl_ColorChanged;
        EditorControl.OpacityChangedEnd += EditorControl_OpacityChangedEnd;
        EditorControl.ThicknessChangedEnd += EditorControl_ThicknessChangedEnd;
        PdfViewer.AnnotationSelected += PdfViewer_AnnotationSelected;
        PdfViewer.AnnotationDeselected += PdfViewer_AnnotationDeselected;
        PdfViewer.PropertyChanged += PdfViewer_PropertyChanged;
#if ANDROID
        Export.Margin = new Thickness(1, 0, 0, 0);
#elif MACCATALYST
        Save.Margin = new Thickness(4, 0, 4, 0);
#elif WINDOWS
        Save.Margin = new Thickness(4, 0, 9, 0);
#endif
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

    #region Text Markup Annotation
    private void PdfViewer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AnnotationMode))
        {
            if (PdfViewer.AnnotationMode == AnnotationMode.None)
            {
                EditorControl.IsVisible = false;
            }
            else
            {
                SetSliderOpacityBasedOnAnnotationSetting();
            }
        }
        else if (e.PropertyName == nameof(PdfViewer.DocumentSource))
        {
            TextMarkupGrid.IsVisible = false;
            EditorControl.IsVisible = false;
            EditOptions.IsVisible = false;
            TextMarkupIcons.IsVisible = true;
            ResetEditorControlAppearance();
        }
    }

    /// <summary>
    /// Sets the opacity of the editor control based on the current annotation setting in the PdfViewer.
    /// </summary>
    private void SetSliderOpacityBasedOnAnnotationSetting()
    {
        if (PdfViewer.AnnotationMode == AnnotationMode.Highlight)
        {
            EditorControl.EditorOpacity = PdfViewer.AnnotationSettings.Highlight.Opacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Underline)
        {
            EditorControl.EditorOpacity = PdfViewer.AnnotationSettings.Underline.Opacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.StrikeOut)
        {
            EditorControl.EditorOpacity = PdfViewer.AnnotationSettings.StrikeOut.Opacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Squiggly)
        {
            EditorControl.EditorOpacity = PdfViewer.AnnotationSettings.Squiggly.Opacity;
        }
    }

    /// <summary>
    /// Handles the event when the opacity of the editor control changes and updates the opacity of the selected annotation.
    /// If no annotation is selected, it sets the default opacity for text markups.
    /// </summary>
    private void EditorControl_OpacityChangedEnd(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Opacity = (float)EditorControl.EditorOpacity;
        }
        else
        {
            SetDefaultOpacityForTextMarkups();
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
            if (SelectedAnnotation is InkAnnotation ink)
            {
                ink.BorderWidth = (int)EditorControl.EditorThickness;
            }
        }
    }

    /// <summary>
    /// Sets the default opacity for text markups in the PdfViewer's annotation settings based on the current editor control's opacity.
    /// </summary>
    private void SetDefaultOpacityForTextMarkups()
    {
        if (PdfViewer.AnnotationMode == AnnotationMode.Highlight)
        {
            PdfViewer.AnnotationSettings.Highlight.Opacity = (float)EditorControl.EditorOpacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Underline)
        {
            PdfViewer.AnnotationSettings.Underline.Opacity = (float)EditorControl.EditorOpacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.StrikeOut)
        {
            PdfViewer.AnnotationSettings.StrikeOut.Opacity = (float)EditorControl.EditorOpacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Squiggly)
        {
            PdfViewer.AnnotationSettings.Squiggly.Opacity = (float)EditorControl.EditorOpacity;
        }
    }

    /// <summary>
    /// Handles the event when the color of the editor control changes and updates the color of the selected annotation.
    /// If no annotation is selected, it sets the default color for text markups.
    /// </summary>
    private void EditorControl_ColorChanged(object sender, ColorChangedEventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.Color = Color.FromArgb(e.ColorCode);
        }
        else
        {
            SetDefaultColorForTextMarkups(Color.FromArgb(e.ColorCode));
        }
    }

    /// <summary>
    /// Sets the default color for text markups in the PdfViewer's annotation settings based on the specified color.
    /// </summary>
    /// <param name="color">The color to be set for the text markups.</param>
    private void SetDefaultColorForTextMarkups(Color color)
    {
        if (PdfViewer.AnnotationMode == AnnotationMode.Highlight)
        {
            PdfViewer.AnnotationSettings.Highlight.Color = color;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Underline)
        {
            PdfViewer.AnnotationSettings.Underline.Color = color;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.StrikeOut)
        {
            PdfViewer.AnnotationSettings.StrikeOut.Color = color;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Squiggly)
        {
            PdfViewer.AnnotationSettings.Squiggly.Color = color;
        }
    }

    /// <summary>
    /// Handles the event when an annotation is deselected in the PdfViewer.
    /// </summary>
    private void PdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = null;
        EditOptions.IsVisible = false;
        TextMarkupIcons.IsVisible = true;
        ResetEditorControlAppearance();
    }

    /// <summary>
    /// Resets the appearance of the editor control to its default state.
    /// </summary>
    private void ResetEditorControlAppearance()
    {
        EditorControl.ThicknessSliderLayOut.IsVisible = false;
        EditorControl.ColorPaletteLayOut.IsVisible = true;
        EditorControl.ColorOpacitySeparator.IsVisible = true;
        EditorControl.OpacityThicknessSeparator.IsVisible = false;
        EditorControl.HeightRequest = 123f;
    }

    /// <summary>
    /// Handles the event when an annotation is selected in the PdfViewer.
    /// </summary>
    private void PdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = e.Annotation;
        UpdateEditorControlAppearance();
        SetAnnotationLockModes();
        ResetOpacitySlider();
        ShowEditOptionsAndHideMarkup();
        ResetThicknessSlider();
    }

    /// <summary>
    /// Resets the thickness slider in the editor control to match the border width of the selected shape or ink annotation.
    /// </summary>
    private void ResetThicknessSlider()
    {
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
    /// Updates the appearance of the editor control based on the selected annotation type.
    /// </summary>
    private void UpdateEditorControlAppearance()
    {
        if (SelectedAnnotation is UnderlineAnnotation || SelectedAnnotation is StrikeOutAnnotation ||
                SelectedAnnotation is SquigglyAnnotation || SelectedAnnotation is HighlightAnnotation)
        {
            ResetEditorControlAppearance();
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
            EditorControl.ThicknessSliderLayOut.IsVisible = true;
            EditorControl.ColorPaletteLayOut.IsVisible = true;
            EditorControl.ColorOpacitySeparator.IsVisible = true;
            EditorControl.OpacityThicknessSeparator.IsVisible = true;
            EditorControl.HeightRequest = 200f;
        }
    }

    /// <summary>
    /// Shows the edit options and hides the markup controls in the UI.
    /// </summary>
    private void ShowEditOptionsAndHideMarkup()
    {
        if (!TextMarkupGrid.IsVisible)
        {
            TextMarkupGrid.IsVisible = true;
        }
        EditOptions.IsVisible = true;
        TextMarkupIcons.IsVisible = false;
        if (EditorControl.IsVisible)
            EditorControl.IsVisible = false;
    }

    /// <summary>
    /// Resets the opacity slider in the editor control to match the opacity of the selected annotation, if available.
    /// </summary>
    private void ResetOpacitySlider()
    {
        if (SelectedAnnotation != null)
        {
            EditorControl.EditorOpacity = SelectedAnnotation.Opacity;
        }
    }

    /// <summary>
    /// Sets the visibility of lock and unlock options based on the lock mode of the selected annotation.
    /// </summary>
    private void SetAnnotationLockModes()
    {
        Lock.IsVisible = SelectedAnnotation.IsLocked ? false : true;
        Unlock.IsVisible = SelectedAnnotation.IsLocked ? true : false;
    }

    /// <summary>
    /// Handles the event when the "Highlight" button is clicked, enabling the highlight annotation mode.
    /// </summary>
    private void HighlightClicked(object sender, EventArgs e)
    {
        PdfViewer.AnnotationMode = AnnotationMode.Highlight;
        EditorControl.IsVisible = true;
        if (EditorControl.EditorOpacity != PdfViewer.AnnotationSettings.Highlight.Opacity)
            SetSliderOpacityBasedOnAnnotationSetting();
    }

    /// <summary>
    /// Handles the event when the "Underline" button is clicked, enabling the Underline annotation mode.
    /// </summary>
    private void UnderlineClicked(object sender, EventArgs e)
    {
        PdfViewer.AnnotationMode = AnnotationMode.Underline;
        EditorControl.IsVisible = true;
        if (EditorControl.EditorOpacity != PdfViewer.AnnotationSettings.Underline.Opacity)
            SetSliderOpacityBasedOnAnnotationSetting();
    }

    /// <summary>
    /// Handles the event when the "Strikeout" button is clicked, enabling the Strikeout annotation mode.
    /// </summary>
    private void StrikeOutClicked(object sender, EventArgs e)
    {
        PdfViewer.AnnotationMode = AnnotationMode.StrikeOut;
        EditorControl.IsVisible = true;
        if (EditorControl.EditorOpacity != PdfViewer.AnnotationSettings.StrikeOut.Opacity)
            SetSliderOpacityBasedOnAnnotationSetting();
    }

    /// <summary>
    /// Handles the event when the "Squiggly" button is clicked, enabling the Squiggly annotation mode.
    /// </summary>
    private void SquigglyClicked(object sender, EventArgs e)
    {
        PdfViewer.AnnotationMode = AnnotationMode.Squiggly;
        EditorControl.IsVisible = true;
        if (EditorControl.EditorOpacity != PdfViewer.AnnotationSettings.Squiggly.Opacity)
            SetSliderOpacityBasedOnAnnotationSetting();
    }

    /// <summary>
    /// Handles the event when the "Show Text Markup" button is clicked, toggling the visibility of text markup controls and editor.
    /// </summary>
    private void ShowTextMarkup_Clicked(object sender, EventArgs e)
    {
        if (TextMarkupGrid.IsVisible && EditOptions.IsVisible)
        {
            TextMarkupIcons.IsVisible = true;
            EditorControl.IsVisible = false;
            EditOptions.IsVisible = false;
            return;
        }
        TextMarkupGrid.IsVisible = !TextMarkupGrid.IsVisible;
        if (!TextMarkupGrid.IsVisible)
        {
            PdfViewer.AnnotationMode = AnnotationMode.None;
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
    /// Handles the event when the "Lock/Unlock" button is clicked, toggling the lock status of the selected annotation.
    /// </summary>
    private void LockUnlock_Clicked(object sender, EventArgs e)
    {
        if (SelectedAnnotation != null)
        {
            SelectedAnnotation.IsLocked = !SelectedAnnotation.IsLocked;
        }
        SetAnnotationLockModes();
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
        // Copy the file to the Directory
        using FileStream outputStream = File.Create(targetFile);
        await inputStream.CopyToAsync(outputStream);
        await DisplayAlert("Annotations exported", "The annotations are exported to the file " + targetFile, "OK");
    }
    
    private void EditOption_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible) && !EditOptions.IsVisible)
        {
            EditorControl.IsVisible = false;
        }
    }
    #endregion
}
