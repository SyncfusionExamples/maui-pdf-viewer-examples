using Syncfusion.Telemetry;

namespace PdfViewerExample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		Telemetry.IsTelemetryEnabled = false;
		InitializeComponent();
	}
}

