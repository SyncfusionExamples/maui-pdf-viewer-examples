using Syncfusion.Maui.PdfViewer;

namespace ElectronicSignature;

public partial class MainPage : ContentPage
{
    Annotation? selectedAnnotation;
    public MainPage()
	{
		InitializeComponent();
        pdfViewer.PropertyChanged += PdfViewer_PropertyChanged;
    }
    private void PdfViewer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Syncfusion.Maui.PdfViewer.SfPdfViewer.AnnotationMode))
            signatureButton.IsVisible = pdfViewer.AnnotationMode != AnnotationMode.Signature;
    }

    private async void saveAsButton_Clicked(object sender, EventArgs e)
    {
        Stream outStream = new MemoryStream();
        await pdfViewer.SaveDocumentAsync(outStream);
        try
        {
            if (viewModel.FileData != null)
            {
                string? filePath = await FileService.SaveAsAsync(viewModel.FileData.FileName, outStream);
                await Application.Current!.MainPage!.DisplayAlert("File saved", $"The file is saved to {filePath}", "OK");
            }
        }
        catch (Exception exception)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"The file is not saved. {exception.Message}", "OK");
        }
    }

    void ToggleDeleteOptionVisibility(bool isVisible)
    {
        deleteButton.IsVisible = isVisible;
    }

    private void deleteButton_Clicked(object sender, EventArgs e)
    {
        if (selectedAnnotation != null)
            pdfViewer.RemoveAnnotation(selectedAnnotation);
        ToggleDeleteOptionVisibility(false);
    }

    private void pdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
    {
        selectedAnnotation = e.Annotation;
        ToggleDeleteOptionVisibility(true);
    }

    private void pdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
    {
        if (e.Annotation == selectedAnnotation)
            selectedAnnotation = null;
        ToggleDeleteOptionVisibility(false);
    }
   
    private void signatureButton_Clicked(object sender, EventArgs e)
    {
        pdfViewer.AnnotationMode = AnnotationMode.Signature;
    }
}