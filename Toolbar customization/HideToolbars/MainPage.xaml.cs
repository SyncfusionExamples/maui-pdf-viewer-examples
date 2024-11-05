using System.Reflection;

namespace PdfViewerDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            PdfViewer.DocumentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("PdfViewerDemo.Assets.pdf_succinctly.pdf");
        }
    }
}