using Syncfusion.Telemetry;

namespace OpenLocalFile;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		Telemetry.IsTelemetryEnabled = false;
		InitializeComponent();
	}
}

