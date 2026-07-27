using Syncfusion.Telemetry;

namespace OpenURLFile.View;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		Telemetry.IsTelemetryEnabled = false;
		InitializeComponent();
	}
}

