using Syncfusion.Maui.PdfViewer;

namespace PdfViewerMirroringDemo;

public partial class MainPage : ContentPage
{
    // This flag checks if the property of the first viewer is changed
    bool isViewer1PropertyChanged = false;
    // This flag checks if the property of the second viewer is changed
    bool isViewer2PropertyChanged = false;

    public MainPage()
    {
        InitializeComponent();
        
        //Load the same document in both the viewers.
        PdfViewer1.DocumentSource = this.GetType().Assembly.GetManifestResourceStream("PdfViewerMirroringDemo.Assets.PDF_Succinctly.pdf");
        PdfViewer2.DocumentSource = this.GetType().Assembly.GetManifestResourceStream("PdfViewerMirroringDemo.Assets.PDF_Succinctly.pdf");
        
        // Wire the property changed event handlers to communicate if any property change occurs between the viewers.
        PdfViewer1.PropertyChanged += PdfViewer1_PropertyChanged;
        PdfViewer2.PropertyChanged += PdfViewer2_PropertyChanged;
    }

    private void PdfViewer1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // If the property change is already handled by the other PDF viewer, return to avoid recursive calls on property changes.
        if (isViewer2PropertyChanged)
            return;

        // If the property change is related to zoom factor or offset, synchronize to other PDF viewer.
        if (e.PropertyName == "ZoomFactor" || e.PropertyName == "VerticalOffset" || e.PropertyName == "HorizontalOffset")
        {
            isViewer1PropertyChanged = true;
            SynchronizeProperty(PdfViewer1, PdfViewer2, e.PropertyName);
        }
    }

    private void PdfViewer2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // If the property change is already handled by the other PDF viewer, return to avoid recursive calls on property changes.
        if (isViewer1PropertyChanged)
            return;

        // If the property change is related to zoom factor or offset, synchronize to other PDF viewer.
        if (e.PropertyName == "ZoomFactor" || e.PropertyName == "VerticalOffset" || e.PropertyName == "HorizontalOffset")
        {
            isViewer2PropertyChanged = true;
            SynchronizeProperty(PdfViewer2, PdfViewer1, e.PropertyName);
        }
    }

    /// <summary>
    /// Synchronizes the property of one PDF Viewer with the other.
    /// </summary>
    /// <param name="sourceViewer">The source viewer at which the property change occurred.</param>
    /// <param name="destinationViewer">The destination viewer to be synchronized based on the changes in other viewer</param>
    /// <param name="propertyName">Name of the property that needs to be synchronized.</param>
    void SynchronizeProperty(SfPdfViewer sourceViewer, SfPdfViewer destinationViewer, string propertyName)
    {
        if (propertyName == "ZoomFactor")
        {
            // Set the zoom factor of the destination viewer to the zoom factor of the source viewer.
            destinationViewer.ZoomFactor = sourceViewer.ZoomFactor;
        }
        if (propertyName == "VerticalOffset" || propertyName == "HorizontalOffset")
        {
            // Scroll the destination viewer to the offsets of the source viewer.
            destinationViewer.ScrollToOffset(sourceViewer.HorizontalOffset.Value, sourceViewer.VerticalOffset.Value);
            // Reset the property changed flags.
            isViewer1PropertyChanged = isViewer2PropertyChanged = false;
        }
    }

    #region Zoom button click event handlers
    /// <summary>
    /// Occurs when the zoom out button associated with the first viewer is clicked.
    /// </summary>
    private void PdfViewer1_ZoomOutClicked(object sender, EventArgs e)
    {
        // Zoomout the first viewer by 25%.
        PdfViewer1.ZoomFactor -= 0.25;
    }

    /// <summary>
    /// Occurs when the zoom in button associated with the first viewer is clicked.
    /// </summary>
    private void PdfViewer1_ZoomInClicked(object sender, EventArgs e)
    {
        // Zoomin the first viewer by 25%.
        PdfViewer1.ZoomFactor += 0.25;
    }

    /// <summary>
    /// Occurs when the zoom out button associated with the second viewer is clicked.
    /// </summary>
    private void PdfViewer2_ZoomOutClicked(object sender, EventArgs e)
    {
        // Zoomout the second viewer by 25%.
        PdfViewer2.ZoomFactor -= 0.25;
    }

    /// <summary>
    /// Occurs when the zoom in button associated with the second viewer is clicked.
    /// </summary>
    private void PdfViewer2_ZoomInClicked(object sender, EventArgs e)
    {
        // Zoomin the second viewer by 25%.
        PdfViewer2.ZoomFactor += 0.25;
    }
    #endregion
}