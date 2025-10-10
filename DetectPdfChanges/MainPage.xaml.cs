using Syncfusion.Maui.PdfViewer;

namespace DetectPdfChanges
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void PdfViewer_FormFieldValueChanged(object sender, FormFieldValueChangedEventArgs e)
        {
            if (Application.Current != null)
                Application.Current.Windows[0].Page?.DisplayAlert("PDF Edited", $"{e.FormField} value is changed.", "OK");
        }

        private void PdfViewer_AnnotationAdded(object sender, AnnotationEventArgs e)
        {
            if (Application.Current != null)
                Application.Current.Windows[0].Page?.DisplayAlert("PDF Edited", "Annotation is added.", "OK");
        }

        private void PdfViewer_AnnotationEdited(object sender, AnnotationEventArgs e)
        {
            if (Application.Current != null)
                Application.Current.Windows[0].Page?.DisplayAlert("PDF Edited", $"Annotation is edited.", "OK");
        }

        private void PdfViewer_AnnotationRemoved(object sender, AnnotationEventArgs e)
        {
            if(Application.Current != null)
                Application.Current.Windows[0].Page?.DisplayAlert("PDF Edited", $"Annotation is removed.", "OK");
        }
    }
}
