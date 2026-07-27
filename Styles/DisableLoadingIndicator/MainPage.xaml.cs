using Syncfusion.Telemetry;
namespace DisableLoadingIndicator
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
