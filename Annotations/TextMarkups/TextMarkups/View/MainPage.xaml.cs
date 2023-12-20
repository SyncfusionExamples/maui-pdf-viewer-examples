using Microsoft.Maui;
using Microsoft.Maui.Platform;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Maui.Sliders;
using System.Windows.Input;

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
        textMarkupEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Highlight.Color.Alpha;
        if (this.BindingContext is PdfViewerViewModel viewModel)
        {
            viewModel.SelectedOpacity = textMarkupEditor.SelectedOpacity;
        }
        textMarkupEditor.ColorChanged += FreeTextEditor_FontColorChanged;
        textMarkupEditor.OpacityChanged += FreeTextEditor_OpacityChanged;
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
                textMarkupEditor.IsVisible = false;
            }
            else
            {
                SetSliderOpacityBasedOnAnnotationSetting();
            }
        }
        else if (e.PropertyName == nameof(PdfViewer.DocumentSource))
        {
            TextMarkupGrid.IsVisible = false;
            textMarkupEditor.IsVisible = false;
            TextMarkupIcons.IsVisible = true;
        }
    }

    /// <summary>
    /// Sets the opacity of the editor control based on the current annotation setting in the PdfViewer.
    /// </summary>
    private void SetSliderOpacityBasedOnAnnotationSetting()
    {
        if (PdfViewer.AnnotationMode == AnnotationMode.Highlight)
        {
            textMarkupEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Highlight.Opacity;
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedOpacity = PdfViewer.AnnotationSettings.Highlight.Opacity;
            }
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Underline)
        {
            textMarkupEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Underline.Opacity;
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedOpacity = PdfViewer.AnnotationSettings.Underline.Opacity;
            }
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.StrikeOut)
        {
            textMarkupEditor.SelectedOpacity = PdfViewer.AnnotationSettings.StrikeOut.Opacity;
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedOpacity = PdfViewer.AnnotationSettings.StrikeOut.Opacity;
            }
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Squiggly)
        {
            textMarkupEditor.SelectedOpacity = PdfViewer.AnnotationSettings.Squiggly.Opacity;
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedOpacity = PdfViewer.AnnotationSettings.Squiggly.Opacity;
            }
        }
    }

    /// <summary>
    /// Handles the event when the opacity of the editor control changes and updates the opacity of the selected annotation.
    /// If no annotation is selected, it sets the default opacity for text markups.
    /// </summary>
    private void FreeTextEditor_OpacityChanged(object? sender, float e)
    {
        if (SelectedAnnotation != null && (SelectedAnnotation is HighlightAnnotation || SelectedAnnotation is UnderlineAnnotation || SelectedAnnotation is StrikeOutAnnotation || SelectedAnnotation is SquigglyAnnotation))
            SelectedAnnotation.Opacity = e;
        else
            SetDefaultOpacityForTextMarkups(e);
    }

    /// <summary>
    /// Sets the default opacity for text markups in the PdfViewer's annotation settings based on the current editor control's opacity.
    /// </summary>
    private void SetDefaultOpacityForTextMarkups(float opacity)
    {
        if (PdfViewer.AnnotationMode == AnnotationMode.Highlight)
        {
            PdfViewer.AnnotationSettings.Highlight.Opacity = opacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Underline)
        {
            PdfViewer.AnnotationSettings.Underline.Opacity = opacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.StrikeOut)
        {
            PdfViewer.AnnotationSettings.StrikeOut.Opacity = opacity;
        }
        else if (PdfViewer.AnnotationMode == AnnotationMode.Squiggly)
        {
            PdfViewer.AnnotationSettings.Squiggly.Opacity = opacity;
        }
    }

    /// <summary>
    /// Handles the event when the color of the editor control changes and updates the color of the selected annotation.
    /// If no annotation is selected, it sets the default color for text markups.
    /// </summary>
    private void FreeTextEditor_FontColorChanged(object? sender, Color e)
    {
        if (SelectedAnnotation != null && (SelectedAnnotation is HighlightAnnotation || SelectedAnnotation is UnderlineAnnotation || SelectedAnnotation is StrikeOutAnnotation || SelectedAnnotation is SquigglyAnnotation))
            SelectedAnnotation.Color = e;
        else
            SetDefaultColorForTextMarkups(e);
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
        TextMarkupIcons.IsVisible = true;
        Delete.IsVisible = false;
        ColorPalette.IsVisible = false;
        Lock.IsVisible = false;
        Unlock.IsVisible = false;
        textMarkupEditor.IsVisible = false;
    }

    /// <summary>
    /// Handles the event when an annotation is selected in the PdfViewer.
    /// </summary>
    private void PdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
    {
        SelectedAnnotation = e.Annotation;
        Delete.IsVisible = true;
        Lock.IsVisible = true;
        Unlock.IsVisible = true;
        ColorPalette.IsVisible = true;
        if (SelectedAnnotation is HighlightAnnotation highlight)
        {
            textMarkupEditor.SelectedOpacity = highlight.Opacity;
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedOpacity = highlight.Opacity;
            }
        }
        if (SelectedAnnotation is UnderlineAnnotation underline)
        {
            textMarkupEditor.SelectedOpacity = underline.Opacity;
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedOpacity = underline.Opacity;
            }
        }
        if (SelectedAnnotation is StrikeOutAnnotation strikeout)
        {
            textMarkupEditor.SelectedOpacity = strikeout.Opacity;
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedOpacity = strikeout.Opacity;
            }
        }
        if (SelectedAnnotation is SquigglyAnnotation squiggly)
        {
            textMarkupEditor.SelectedOpacity = squiggly.Opacity;
            if (this.BindingContext is PdfViewerViewModel viewModel)
            {
                viewModel.SelectedOpacity = squiggly.Opacity;
            }
        }
        SetAnnotationLockModes();
        ShowEditOptionsAndHideMarkup();
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
        TextMarkupIcons.IsVisible = false;
        if (textMarkupEditor.IsVisible)
            textMarkupEditor.IsVisible = false;
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
        ColorPalette.IsVisible = true;
    }

    /// <summary>
    /// Handles the event when the "Underline" button is clicked, enabling the Underline annotation mode.
    /// </summary>
    private void UnderlineClicked(object sender, EventArgs e)
    {
        PdfViewer.AnnotationMode = AnnotationMode.Underline;
        ColorPalette.IsVisible = true;
    }

    /// <summary>
    /// Handles the event when the "Strikeout" button is clicked, enabling the Strikeout annotation mode.
    /// </summary>
    private void StrikeOutClicked(object sender, EventArgs e)
    {
        PdfViewer.AnnotationMode = AnnotationMode.StrikeOut;
        ColorPalette.IsVisible = true;
    }

    /// <summary>
    /// Handles the event when the "Squiggly" button is clicked, enabling the Squiggly annotation mode.
    /// </summary>
    private void SquigglyClicked(object sender, EventArgs e)
    {
        PdfViewer.AnnotationMode = AnnotationMode.Squiggly;
        ColorPalette.IsVisible = true;
    }

    /// <summary>
    /// Handles the event when the "Show Text Markup" button is clicked, toggling the visibility of text markup controls and editor.
    /// </summary>
    private void ShowTextMarkup_Clicked(object sender, EventArgs e)
    {
        if (TextMarkupGrid.IsVisible && EditOptions.IsVisible)
        {
            TextMarkupIcons.IsVisible = true;
            textMarkupEditor.IsVisible = false;
            return;
        }
        TextMarkupGrid.IsVisible = !TextMarkupGrid.IsVisible;
        if (!TextMarkupGrid.IsVisible)
        {
            PdfViewer.AnnotationMode = AnnotationMode.None;
            textMarkupEditor.IsVisible = false;
        }
    }

    /// <summary>
    /// Handles the event when the color palette button is clicked, toggling the visibility of the editor control.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    private void ColorPalette_Clicked(object sender, EventArgs e)
    {
        textMarkupEditor.IsVisible = !textMarkupEditor.IsVisible;
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
    
    private void EditOption_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible) && !EditOptions.IsVisible)
        {
            textMarkupEditor.IsVisible = false;
        }
    }
    #endregion
}
