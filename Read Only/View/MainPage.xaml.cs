namespace PdfViewerExample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private void PdfViewer_DocumentLoaded(object sender, EventArgs e)
    {
        LockAllAnnotationsAndFields();
    }

    // Locks all the annotations and form fields in the document.
    void LockAllAnnotationsAndFields()
    {
        PdfViewer.AnnotationSettings.IsLocked = true;
        if (PdfViewer.FormFields?.Count > 0)
        {
            foreach (var field in PdfViewer.FormFields)
            {
                field.ReadOnly = true;
            }
        }
    }
}

