using System.Reflection;

namespace ReplaceToolbarItem
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Convert the PDF to a stream.
            Stream? stream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("ReplaceToolbarItem.Assets.PDF_Succinctly.pdf");
            // Load the stream into the pdfViewer control.
            pdfViewer.LoadDocument(stream);
        }

        private void PdfViewerDocumentLoaded(object sender, EventArgs e)
        {

#if ANDROID || IOS

            // Get the "MoreOptions" toolbar item from the top toolbar.
            var item = pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem");

            // Get the index of the "MoreOptions" toolbar item from the top toolbar.
            var index = (item?.Index != null) ? (int)item.Index : -1;

            if (item != null)
                // Remove the "MoreOptions" toolbar item from the top toolbar.
                pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Remove(item);

            // Creating a new print button to replace the "MoreOptions" toolbar item.
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

            // Replace the print button at the index of the "MoreOptions" toolbar item. 
            pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(index, new Syncfusion.Maui.PdfViewer.ToolbarItem(printButton, "printButton"));
#endif

        }
    }
}
