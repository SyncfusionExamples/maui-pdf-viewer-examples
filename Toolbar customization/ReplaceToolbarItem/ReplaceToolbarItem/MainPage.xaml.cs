using System.Reflection;

namespace ReplaceToolbarItem
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Stream stream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("ReplaceToolbarItem.Assets.PDF_Succinctly.pdf");
            pdfViewer.LoadDocument(stream);
        }
        private void PdfViewerDocumentLoaded(object sender, EventArgs e)
        {

#if ANDROID || IOS
            var index = (int)pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem")?.Index;
            var item = pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.GetByName("MoreItem");
            if (item != null)
                pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Remove(item);
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
            pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(index, new Syncfusion.Maui.PdfViewer.ToolbarItem(printButton, "printButton"));
#endif

        }
    }
}
