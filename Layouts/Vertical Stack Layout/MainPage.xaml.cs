namespace PdfViewerInStackLayout
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when the size of the vertical stack layout changes.
        /// </summary>
        private void VStackLayout_SizeChanged(object sender, EventArgs e)
        {
            // Check if the sender is a vertical stack layout.
            if (sender is VerticalStackLayout stackLayout)
            {
                // Check if the PDF Viewer is set to fill the vertical stack layout.
                if (PdfViewer.VerticalOptions == LayoutOptions.Fill)
                {
                    // Set the height of the PDF Viewer to the height of the vertical stack layout.
                    PdfViewer.HeightRequest = stackLayout.Height;
                }
            }
        }
    }
}
