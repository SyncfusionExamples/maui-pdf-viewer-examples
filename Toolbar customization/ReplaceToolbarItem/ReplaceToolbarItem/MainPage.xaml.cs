using System.Reflection;

namespace ReplaceToolbarItem
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Converting pdf to stream.
            Stream stream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("ReplaceToolbarItem.Assets.PDF_Succinctly.pdf");
            // Loading the stream into pdfViewer control.
            pdfViewer.LoadDocument(stream);
        }

        private void PdfViewerDocumentLoaded(object sender, EventArgs e)
        {

#if ANDROID || IOS
            // Getting the index value of the more item toolbar item from the top toolbar.
            var index = (int)pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem")?.Index;

            // Get the more item from the top toolbar.
            var item = pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem");
            if (item != null)
                // Remove the more item from the top toolbar.
                pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Remove(item);

            // Creating a new print button to replace the more item.
            Button printButton = new Button
            {
                Text = "\ue77f",
                FontSize = 24,
                FontFamily = "MauiMaterialAssets",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Colors.Transparent,
                TextColor = Color.FromArgb("#49454F"),
                IsEnabled = true,
                Padding = 10,

            };

            // Replacing the print button in the index of the more item. 
            pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(index, new Syncfusion.Maui.PdfViewer.ToolbarItem(printButton, "printButton"));
#endif

        }
    }
}
