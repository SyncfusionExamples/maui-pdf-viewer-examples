using Syncfusion.Telemetry;
namespace BusyIndicatorStyle;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
        Telemetry.IsTelemetryEnabled = false;
		InitializeComponent();
	}

    private void Load_Clicked(object sender, EventArgs e)
    {
        pdfViewer.DocumentSource = this.GetType().Assembly.GetManifestResourceStream("BusyIndicatorStyle.Resources.Raw.pdf_succinctly.pdf");
    }
}

