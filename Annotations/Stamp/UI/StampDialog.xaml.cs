using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace Stamp;

public partial class StampDialog : ContentView
{
    private StampType stampType { get; set; }
    private Stream imageStream { get; set; }

    public StampDialog()
    {
        InitializeComponent();

    }

    private void OnCloseButtonClicked(object sender, EventArgs e)
    {
        this.IsVisible = false;
    }

    /// <summary>
    /// Event handler for the tap event on image buttons representing different stamp types. 
    /// Determines the selected stamp type or custom image and sends a corresponding message using MessagingCenter.
    /// </summary>
    /// <param name="sender">The sender object (image button).</param>
    /// <param name="e">The EventArgs containing information about the tap event.</param>
    private async void OnImageTapped(object sender, EventArgs e)
    {
        if (sender == Approved)
        {
            stampType = StampType.Approved;
            imageStream = null;
        }
        else if (sender == Draft)
        {
            stampType = StampType.Draft;
            imageStream = null;
        }
        else if (sender == Confidential)
        {
            stampType = StampType.Confidential;
            imageStream = null;
        }
        else if (sender == NotApproved)
        {
            stampType = StampType.NotApproved;
            imageStream = null;
        }
        else if (sender == Dotnet)
        {
            Stream streamImage = await GetImage("Stamp.Assets.Images.dotnet.jpg");
            imageStream = streamImage;
        }
        else if (sender == Download)
        {
            Stream stream = await GetImage("Stamp.Assets.Images.download.png");
            imageStream = stream;
        }
        if (imageStream == null)
        {
            MessagingCenter.Send<StampDialog, StampType>(this, "Built-inStamp", stampType);
        }
        else
        {
            MessagingCenter.Send<StampDialog, Stream>(this, "CustomStamp", imageStream);
        }
        this.IsVisible = false;
    }

    private Task<Stream> GetStreamFromImageSourceAsync(StreamImageSource streamImage, CancellationToken cancellationToken)
    {
        if (streamImage.Stream != null)
        {
            return streamImage.Stream(cancellationToken);
        }
        return null;
    }

    private async Task<Stream> GetImage(string imageFile)
    {
        Image image = new Image();
        image.Source = Microsoft.Maui.Controls.ImageSource.FromResource(imageFile, typeof(App).GetTypeInfo().Assembly);
        Stream stream = null;
        if (image.Source is StreamImageSource streamImage)
        {
            CancellationToken cancellationToken = default(CancellationToken);
            stream = await GetStreamFromImageSourceAsync(streamImage, cancellationToken);
        }
        return stream;
    }
}