namespace AutoSavePDFinAWS
{
    public partial class MainPage : ContentPage
    {

        /// <summary>
        /// ViewModel instance that handles business logic and data binding
        /// </summary>
        private PdfViewerViewModel ViewModel;

        public MainPage()
        {
            InitializeComponent();

            // Initialize ViewModel and set up data binding context
            ViewModel = new PdfViewerViewModel();
            BindingContext = ViewModel;

            // Provide PDF Viewer reference to ViewModel for document operations
            ViewModel.PdfViewer = PdfViewer;
        }

        /// <summary>
        /// Handles the DocumentLoaded event when a PDF is successfully loaded into the viewer.
        /// Updates the ViewModel to reflect that a document is now available.
        /// </summary>
        /// <param name="sender">The PDF Viewer control that raised the event</param>
        /// <param name="e">Event arguments</param>
        private void PdfViewer_DocumentLoaded(object sender, EventArgs e)
        {
            // Notify ViewModel that a document has been loaded
            ViewModel.IsDocumentLoaded = true;
        }

        /// <summary>
        /// Handles the DocumentUnloaded event when a PDF is unloaded from the viewer.
        /// Updates the ViewModel to reflect that no document is currently loaded.
        /// </summary>
        /// <param name="sender">The PDF Viewer control that raised the event</param>
        /// <param name="e">Event arguments</param>
        private void PdfViewer_DocumentUnloaded(object sender, EventArgs e)
        {
            // Notify ViewModel that the document has been unloaded
            ViewModel.IsDocumentLoaded = false;
        }

        /// <summary>
        /// Handles the AnnotationAdded event when a new annotation is added to the PDF.
        /// Triggers the auto-save mechanism if enabled.
        /// </summary>
        /// <param name="sender">The PDF Viewer control that raised the event</param>
        /// <param name="e">Event arguments containing annotation details</param>
        private void PdfViewer_AnnotationAdded(object sender, Syncfusion.Maui.PdfViewer.AnnotationEventArgs e)
        {
            // Trigger auto-save or notification update for document edit
            ViewModel.OnDocumentEdited();
        }

        /// <summary>
        /// Handles the AnnotationRemoved event when an annotation is deleted from the PDF.
        /// Triggers the auto-save mechanism if enabled.
        /// </summary>
        /// <param name="sender">The PDF Viewer control that raised the event</param>
        /// <param name="e">Event arguments containing annotation details</param>
        private void PdfViewer_AnnotationRemoved(object sender, Syncfusion.Maui.PdfViewer.AnnotationEventArgs e)
        {
            // Trigger auto-save or notification update for document edit
            ViewModel.OnDocumentEdited();
        }

        /// <summary>
        /// Handles the AnnotationEdited event when an existing annotation is modified.
        /// Triggers the auto-save mechanism if enabled.
        /// </summary>
        /// <param name="sender">The PDF Viewer control that raised the event</param>
        /// <param name="e">Event arguments containing annotation details</param>
        private void PdfViewer_AnnotationEdited(object sender, Syncfusion.Maui.PdfViewer.AnnotationEventArgs e)
        {
            // Trigger auto-save or notification update for document edit
            ViewModel.OnDocumentEdited();
        }

        /// <summary>
        /// Handles the FormFieldValueChanged event when a form field value is modified.
        /// Triggers the auto-save mechanism if enabled.
        /// </summary>
        /// <param name="sender">The PDF Viewer control that raised the event</param>
        /// <param name="e">Event arguments containing form field details</param>
        private void PdfViewer_FormFieldValueChanged(object sender, Syncfusion.Maui.PdfViewer.FormFieldValueChangedEventArgs e)
        {
            // Trigger auto-save or notification update for document edit
            ViewModel.OnDocumentEdited();
        }
    }
}
