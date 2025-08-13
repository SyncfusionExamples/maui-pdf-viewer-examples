namespace PDFAutoSaveEdits
{
    public partial class MainPage : ContentPage
    {
        private ViewModel ViewModel;

        public MainPage()
        {
            InitializeComponent();
            ViewModel = new ViewModel();
            BindingContext = ViewModel;
            ViewModel.PdfViewer = PdfViewer;
        }

        private void PdfViewer_DocumentLoaded(object sender, EventArgs e)
        {
            ViewModel.IsDocumentLoaded = true;
        }

        private void PdfViewer_DocumentUnloaded(object sender, EventArgs e)
        {
            ViewModel.IsDocumentLoaded = false;
        }

        private void PdfViewer_AnnotationAdded(object sender, Syncfusion.Maui.PdfViewer.AnnotationEventArgs e)
        {
            ViewModel.OnDocumentEdited();
        }

        private void PdfViewer_AnnotationRemoved(object sender, Syncfusion.Maui.PdfViewer.AnnotationEventArgs e)
        {
            ViewModel.OnDocumentEdited();
        }

        private void PdfViewer_AnnotationEdited(object sender, Syncfusion.Maui.PdfViewer.AnnotationEventArgs e)
        {
            ViewModel.OnDocumentEdited();
        }

        private void PdfViewer_FormFieldValueChanged(object sender, Syncfusion.Maui.PdfViewer.FormFieldValueChangedEventArgs e)
        {
            ViewModel.OnDocumentEdited();
        }
    }
}