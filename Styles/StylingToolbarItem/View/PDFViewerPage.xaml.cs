using System.Reflection;

namespace StylingToolbarItem;

public partial class PDFViewerPage : ContentPage
{
    Button? fileOpenButton;
    public PDFViewerPage()
    {
        InitializeComponent();

        //Accessing the PDF document that is added as embedded resource as stream.
        Stream? documentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("StylingToolbarItem.Assets.PDF_Succinctly.pdf");

        // Assigning stream to "DocumentSource" property.
        PdfViewer.DocumentSource = documentStream;

        // Calling "AddFileOperationsToolbarItems" for to add save and open options toolbar item in toolbar
        AddFileOperationsToolbarItem();
    }

    void AddFileOperationsToolbarItem()
    {
        // Create new open button
        fileOpenButton = new Button
        {
            Text = "\ue712", // Set button text
            FontSize = 24, // Set button text font size
            FontFamily = "MauiMaterialAssets", // Set button text font family
            BackgroundColor = Colors.Transparent, // Set background for the button
            BorderColor = Colors.Transparent, // Set border color for the button
            CornerRadius = 5, // Set corner radius of the button
            Opacity = 1,
            IsEnabled = true
        };

        //Subscription of click event for the open file button
        fileOpenButton.Clicked += FileOpenButton_Clicked;

        //Set color based on theme.
        fileOpenButton.SetAppThemeColor(Button.TextColorProperty,
        Color.FromArgb("#49454F"),
        Color.FromArgb("#CAC4D0"));

        // Set the tooltip text on hover
        ToolTipProperties.SetText(fileOpenButton, "OpenFile");

#if !WINDOWS && !MACCATALYST
        // Inserting open file option button as toolbar item in the top toolbar for the mobile platform.
        PdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "OpenFile"));
#else
        // Inserting open file option button as toolbar item in the primary toolbar for the desktop platform.
        PdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "OpenFile"));
#endif
    }

    private void FileOpenButton_Clicked(object? sender, EventArgs e)
    {
        OpenDocument();
    }

    async void OpenDocument()
    {
        //Create file picker with file type as PDF.
        FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.iOS, new[] { "public.pdf" } },
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.WinUI, new[] { "pdf" } },
                        { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                    });

        //Provide picker title if required.
        PickOptions options = new()
        {
            PickerTitle = "Please select a PDF file",
            FileTypes = pdfFileType,
        };
        await PickAndShow(options);
    }


    /// <summary>
    /// Picks the file from local storage and store as stream.
    /// </summary>
    public async Task PickAndShow(PickOptions options)
    {
        try
        {
            //Pick the file from local storage.
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                //Store the resultant PDF as stream.
                Stream documentStream = await result.OpenReadAsync();
                PdfViewer.LoadDocument(documentStream);
            }
        }
        catch (Exception ex)
        {
            //Display error when file picker failed to open files.
            string message;
            if (ex != null && string.IsNullOrEmpty(ex.Message) == false)
                message = ex.Message;
            else
                message = "File open failed.";
            Application.Current?.Windows[0]?.Page?.DisplayAlert("Error", message, "OK");
        }
    }   
}