namespace SignatureDemo;

public partial class SignatureCaptureView : ContentView
{
    public event EventHandler<ImageSource?>? SignatureCreated;

    public SignatureCaptureView()
	{
		InitializeComponent();
	}

    private void OnClearClicked(object sender, EventArgs e)
    {
        signaturePad.Clear();
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        this.IsVisible = false;
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        ImageSource? imageSource = signaturePad.ToImageSource();
        // Trigger the SignatureCreated event
        SignatureCreated?.Invoke(this, imageSource);
        this.IsVisible = false;
    }
}