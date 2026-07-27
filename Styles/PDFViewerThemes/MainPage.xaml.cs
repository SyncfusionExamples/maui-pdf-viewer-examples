using Syncfusion.Telemetry;
namespace PDFViewerThemes
{
    public partial class MainPage : ContentPage
    {      
        public MainPage()
        {
            Telemetry.IsTelemetryEnabled = false;
            InitializeComponent();
        }       
    }
}
