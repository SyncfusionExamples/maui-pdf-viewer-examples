using Microsoft.Maui.Controls;
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
            // Call a custom method to replace the default toolbar items in the PDF viewer
            ReplaceToolbarItem();
        }

        private void ReplaceToolbarItem()
        {
#if ANDROID || IOS

            // Retrieve the top toolbar using its name"
            var toolbar = pdfViewer.Toolbars?.GetByName("TopToolbar");

            // Retrieve the toolbar item named "MoreOptions" from the "TopToolbar"
            var item = toolbar?.Items?.GetByName("MoreItem");

            // Obtain the index value of the retrieved toolbar item
            var index = (item?.Index != null) ? (int)item.Index : -1;

            if (item != null)
                // Remove the "MoreOptions" toolbar item from the top toolbar.
                pdfViewer.Toolbars?.GetByName("TopToolbar")?.Items?.Remove(item);

            // Create a new print button to replace the "MoreOptions" toolbar item.
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

            // Add a click event handler to the newly created button for initiating document printing
            printButton.Clicked += (s, e) =>
            {
                // The PrintDocument method contains the code for printing the document.
                pdfViewer.PrintDocument();
            };

            // Replace the print button at the index of the "MoreOptions" toolbar item. 
            toolbar?.Items?.Insert(index, new Syncfusion.Maui.PdfViewer.ToolbarItem(printButton, "printButton"));
#endif

        }
    }
}
