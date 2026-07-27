using System.Reflection;
using Syncfusion.Telemetry;

namespace PdfViewerDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            Telemetry.IsTelemetryEnabled = false;
            InitializeComponent();
            PdfViewer.DocumentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("PdfViewerDemo.Assets.pdf_succinctly.pdf");
        }
    }
}