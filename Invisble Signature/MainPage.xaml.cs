using Syncfusion.Telemetry;

namespace InvisbleSignatureDemo
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