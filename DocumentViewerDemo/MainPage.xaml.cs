using Syncfusion.Maui.Core;
using Syncfusion.Maui.PdfViewer;

namespace DocumentViewerDemo
{
    public partial class MainPage : ContentPage
    {
        // Create three instances of SfPdfViewer for displaying PDF documents
        SfPdfViewer pdfViewer = new SfPdfViewer();
        SfPdfViewer pdfViewer1 = new SfPdfViewer();
        SfPdfViewer pdfViewer2 = new SfPdfViewer();
        SfPdfViewer pdfViewer3 = new SfPdfViewer();
        SfPdfViewer pdfViewer4 = new SfPdfViewer();
        SfPdfViewer pdfViewer5 = new SfPdfViewer();
        public MainPage()
        {
            InitializeComponent();

            AddSaveOptionToolbarItems(pdfViewer);
            AddSaveOptionToolbarItems(pdfViewer1);
            AddSaveOptionToolbarItems(pdfViewer2);
            AddSaveOptionToolbarItems(pdfViewer3);
            AddSaveOptionToolbarItems(pdfViewer4);
            AddSaveOptionToolbarItems(pdfViewer5);
            // Set the zoom mode of three PDF viewers to fit the width of the container
            pdfViewer.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer1.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer2.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer3.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer4.ZoomMode = ZoomMode.FitToWidth;
            pdfViewer5.ZoomMode = ZoomMode.FitToWidth;
        }

        void AddSaveOptionToolbarItems(SfPdfViewer pdfViewer)
        {

            // Create new save button
            Button fileSaveButton = new Button
            {
                Text = "\ue75f", // Set button text
                FontSize = 24, // Set button text font size
                FontFamily = "MauiMaterialAssets", // Set button text font family
                BackgroundColor = Colors.Transparent, // Set background for the button
                BorderColor = Colors.Transparent, // Set border color for the button
                TextColor = Color.FromArgb("#49454F"), // Set button text color
                CornerRadius = 5 // Set corner radius of the button
            };

            // Subscription of click event for the save button
            fileSaveButton.Clicked += FileSaveButton_Clicked;

            // Set the tooltip text on hover
            ToolTipProperties.SetText(fileSaveButton, "Save");

#if !WINDOWS && !MACCATALYST
            // Inserting save file option button as toolbar item in the top toolbar for the mobile platform.
            pdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
#else
            // Inserting save file option button as toolbar item in the top toolbar for the desktop platform.
            pdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
#endif
        }

        private async void FileSaveButton_Clicked(object? sender, EventArgs e)
        {
            // Create a new memory stream to hold the saved PDF document
            Stream savedStream = new MemoryStream();

            // Asynchronously save the current document content into the memory stream
            await pdfViewer.SaveDocumentAsync(savedStream); 
            
            try
            {
                savedStream.Position = 0;

                // Open the file explorer using the file picker, select a location to save the PDF, save the file to the chosen path, and retrieve the saved file location for display.
                string? filePath = await FileService.SaveAsAsync("Saved.pdf", savedStream);

                // Safely storing the main page of the application.
                var mainPage = Application.Current?.Windows[0].Page;

                if (mainPage != null)
                    // Display the saved file path.
                    await mainPage.DisplayAlert("File saved", $"The file is saved to {filePath}", "OK");
            }
            catch (Exception exception)
            {
                // Safely storing the main page of the application.
                var mainPage = Application.Current?.Windows[0].Page;

                if (mainPage != null)
                    // Display the error message when the file is not saved.
                    await mainPage.DisplayAlert("Error", $"The file is not saved. {exception.Message}", "OK");
            }
        }

        private void SfTabView_Loaded(System.Object sender, System.EventArgs e)
        {

            // Ensure the viewModel is not null before proceeding
            if (viewModel?.PDFDocuments != null)
            {
                // Check if the first tab's header matches "PDF_Succinctly.pdf"
                if (tab1.Header.Equals("PDF_Succinctly.pdf"))
                {
                    pdfViewer.DocumentSource = viewModel.PDFDocuments[0]; // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab1.Content = pdfViewer; // Set the content of tab1 to the pdfViewer.
                }

                // Check if the second tab's header matches "Input.docx"
                if (tab2.Header.Equals("Input.docx"))
                {
                    pdfViewer1.DocumentSource = viewModel.ConvertDocxToPdf(viewModel.PDFDocuments[1]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab2.Content = pdfViewer1; // Set the content of tab1 to the pdfViewer1.
                }

                // Check if the third tab's header matches "Autumn Leaves.jpg"
                if (tab3.Header.Equals("Autumn Leaves.jpg"))
                {
                    pdfViewer2.DocumentSource = viewModel.ConvertImageToPdf(viewModel.PDFDocuments[2]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab3.Content = pdfViewer2; // Set the content of tab1 to the pdfViewer1.
                }


                // Check if the fourth tab's header matches "Template.pptx"
                if (tab4.Header.Equals("Template.pptx"))
                {
                    pdfViewer3.DocumentSource = viewModel.ConvertPptToPdf(viewModel.PDFDocuments[3]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab4.Content = pdfViewer3; // Set the content of tab1 to the pdfViewer1.
                }


                // Check if the fifth tab's header matches "InputTemplate.xlsx"
                if (tab5.Header.Equals("InputTemplate.xlsx"))
                {
                    pdfViewer4.DocumentSource = viewModel.ConvertXlsxToPdf(viewModel.PDFDocuments[4]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab5.Content = pdfViewer4; // Set the content of tab1 to the pdfViewer1.
                }

                // Check if the sixth tab's header matches "Input.xps"
                if (tab6.Header.Equals("Input.xps"))
                {
                    pdfViewer5.DocumentSource = viewModel.ConvertXpsToPdf(viewModel.PDFDocuments[5]); // Assign the stream to the "DocumentSource" property of the PdfViewer control
                    tab6.Content = pdfViewer5; // Set the content of tab1 to the pdfViewer1.
                }
            }
        }
    }
}
