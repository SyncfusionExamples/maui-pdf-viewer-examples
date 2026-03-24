using Syncfusion.Maui.PdfViewer;

namespace DetectPdfChanges
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void PdfViewer_FormFieldValueChanged(object sender, FormFieldValueChangedEventArgs e)
        {
            var page = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (page is null)
                return;

            await page.DisplayAlertAsync("PDF Edited", $"{e.FormField} value is changed.", "OK");
        }

        private async void PdfViewer_AnnotationAdded(object sender, AnnotationEventArgs e)
        {
            var page = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (page is null)
                return;
            await page.DisplayAlertAsync("PDF Edited", "Annotation is added.", "OK");
        }

        private async void PdfViewer_AnnotationEdited(object sender, AnnotationEventArgs e)
        {
            var page = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (page is null)
                return;
            await page.DisplayAlertAsync("PDF Edited", $"Annotation is edited.", "OK");
        }

        private async void PdfViewer_AnnotationRemoved(object sender, AnnotationEventArgs e)
        {
            var page = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (page is null)
                return;
            await page.DisplayAlertAsync("PDF Edited", $"Annotation is removed.", "OK");
        }
    }
}
