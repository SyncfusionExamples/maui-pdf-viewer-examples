namespace Localization
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OpenPdfViewerPage(object sender, EventArgs e)
        {
            // Use empty string as default if SelectedLanguage is null
            var language = viewModel.SelectedLanguage ?? string.Empty;

            var pdfPage = new SfPdfViewerPage(language);
            await Navigation.PushAsync(pdfPage);
        }

    }

}
