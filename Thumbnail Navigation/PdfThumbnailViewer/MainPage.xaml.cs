using PdfThumbnailViewer.ViewModels;

namespace PdfThumbnailViewer;

public partial class MainPage : ContentPage
{
    private PdfThumbnailViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();
        _viewModel = (PdfThumbnailViewModel)BindingContext;
        _viewModel.Initialize(PdfViewer);
        // Load a sample PDF from the embedded resources
        _viewModel.LoadPdfDocument("PdfThumbnailViewer.Assets.sample.pdf");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Clean up resources
        PdfViewer?.UnloadDocument();
    }
}
