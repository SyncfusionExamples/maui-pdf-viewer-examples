using Syncfusion.Maui.PdfViewer;
using Syncfusion.Pdf.Parsing;

namespace SmartFill;

public partial class MainPage : ContentPage
{
    private AIHelper chatService = new AIHelper();

    private bool tapped;
    Animation animation;
    public MainPage()
    {
        InitializeComponent();
        animation = new Animation();
        Clipboard.ClipboardContentChanged += Clipboard_ClipboardContentChanged;
        PdfViewer.DocumentLoaded += PdfViewer_DocumentLoaded;
    }

    private void PdfViewer_DocumentLoaded(object? sender, EventArgs? e)
    {
#if ANDROID || IOS
        MobileCopiedData.IsVisible = true;
#endif
    }

    private async void Clipboard_ClipboardContentChanged(object? sender, EventArgs e)
    {
        string? copiedText = await Clipboard.GetTextAsync();
        StartBubbleAnimation();
        if (string.IsNullOrEmpty(copiedText) == false)
        {
            SubmitForm.IsEnabled = true;
        }
    }

    // Event handler for the SmartFill button click event
    private async void OnSmartFillClicked(object sender, EventArgs e)
    {
        StopBubbleAnimation();
        loadingIndicator.IsRunning = true; // Show loading indicator
        loadingIndicator.IsVisible = true; // Make loading indicator visible
        PdfViewer.Opacity = 0.5; // Dim the PDF viewer to indicate loading
#if ANDROID || IOS
        SmartTools.IsVisible = false;
#endif
        string? copiedTextContent = await Clipboard.GetTextAsync(); // Variable to hold inputData text
        if (copiedTextContent != null)
        {
            string? exportedFormData = GetXFDFAsString(); // Read XFDF content from PDF Viewer as String to provide input to OpenAI
            string? CustomValues = HintValuesforFieldsAsString();
            string prompt = $"Merge the copied text content into the XFDF file content. Hint text: {CustomValues}. " +
                                $"Ensure the copied text content matches the appropriate field names. " +
                                $"Here are the details: " +
                                $"Copied text content: {copiedTextContent}, " +
                                $"XFDF information: {exportedFormData}. " +
                                $"Provide the resultant XFDF directly. " +
                                $"Please follow these conditions: " +
                                $"1. The input data is not directly provided as the field name; you need to think and merge appropriately. " +
                                $"2. When comparing input data and field names, ignore case sensitivity. " +
                                $"3. First, determine the best match for the field name. If there isn’t an exact match, use the input data to find a close match. " +
                                $"4. Remove ```xml and ``` if they are present in the code.";
            string xfdfContent = await chatService.GetChatCompletion(prompt);
            if (string.IsNullOrEmpty(xfdfContent) == false)
                FillPdfForm(xfdfContent);
        }
        loadingIndicator.IsRunning = false; // Stop the loading indicator
        loadingIndicator.IsVisible = false; // Hide the loading indicator
        PdfViewer.Opacity = 1;
    }

    /// <summary>
    /// Fill the PDF form with the XFDF content
    /// </summary>
    /// <param name="xfdfContent"></param>
    public async void FillPdfForm(string xfdfContent)
    {
        using (Stream xfdfStream = new MemoryStream())
        {
            // Create a StreamWriter to write to the XFDF stream
            var writer = new StreamWriter(xfdfStream, leaveOpen: true);
            // Write the XFDF content to the stream
            await writer.WriteAsync(xfdfContent);
            // Flush the stream
            await writer.FlushAsync();
            xfdfStream.Position = 0;
            // Import the XFDF stream to the PDF Viewer
            PdfViewer.ImportFormData(xfdfStream, DataFormat.XFdf, true);
        }
    }


    // Method to read XFDF content from the PDF viewer and return it as a string
    private string GetXFDFAsString()
    {

        MemoryStream xfdfStream = new MemoryStream(); // Create a new memory stream
        PdfViewer.ExportFormData(xfdfStream, DataFormat.XFdf); // Export form data in XFDF format to the stream
        using (xfdfStream) // Use the stream
        {
            if (xfdfStream != null) // Check if the stream is not null
            {
                using (StreamReader reader = new StreamReader(xfdfStream)) // Create a StreamReader to read from the stream
                {
                    return reader.ReadToEnd(); // Read the stream content to the end and return as string
                }
            }
            else
            {
                return ""; // If stream is null, return an empty string
            }
        }
    }

    // This method generates a string with custom data for each form field in a PDF viewer
    private string HintValuesforFieldsAsString()
    {
        string? hintData = null;
        // Loop through each form field in the PDF viewer
        foreach (FormField form in PdfViewer.FormFields)
        {
            // Check if the form field is a ComboBox
            if (form.GetType() == typeof(ComboBoxFormField))
            {
                ComboBoxFormField? combo = form as ComboBoxFormField;
                if (combo != null)
                {
                    // Append ComboBox name and items to the hintData string
                    hintData += "\n" + combo.Name + " : Collection of Items are :";
                    foreach (string item in combo.Items)
                    {
                        hintData += item + ", ";
                    }
                }
            }
            // Check if the form field is a RadioButton
            else if (form.GetType() == typeof(RadioButtonFormField))
            {
                RadioButtonFormField? radio = form as RadioButtonFormField;
                if (radio != null)
                {
                    // Append RadioButton name and items to the hintData string
                    hintData += "\n" + radio.Name + " : Collection of Items are :";
                    foreach (string item in radio.Items)
                    {
                        hintData += item + ", ";
                    }
                }
            }
            // Check if the form field is a ListBox
            else if (form.GetType() == typeof(ListBoxFormField))
            {
                ListBoxFormField? list = form as ListBoxFormField;
                if (list != null)
                {
                    // Append ListBox name and items to the hintData string
                    hintData += "\n" + list.Name + " : Collection of Items are :";
                    foreach (string item in list.Items)
                    {
                        hintData += item + ", ";
                    }
                }
            }
            // Check if the form field name contains 'Date', 'dob', or 'date'
            else if (form.Name.Contains("Date") || form.Name.Contains("dob") || form.Name.Contains("date"))
            {
                // Append instructions for date format to the hintData string
                hintData += "\n" + form.Name + " : Write Date in MMM/dd/YYYY format";
            }
            // Append other form field names to the hintData string
        }

        // Return the hintData string if not null, otherwise return an empty string
        if (hintData != null)
        {
            return hintData;
        }
        return "";
    }

    private void SavePDF(object sender, EventArgs e)
    {
        var filePath = Path.Combine(FileSystem.AppDataDirectory, "SavedSample.pdf");

        var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        PdfViewer.SaveDocument(stream);
        Application.Current?.MainPage?.DisplayAlert("Success", $"Document saved successfully at:\n{filePath}", "OK");

    }

    /// <summary>
    /// Print the PDF document
    /// </summary>
    private void PrintPDF(object sender, EventArgs e)
    {
        PdfViewer.PrintDocument();
    }

    private async void SetClipboardText(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.Text = "\ue70c";
            switch (button.AutomationId)
            {
                case "CopiedButton1":
                    await Clipboard.SetTextAsync(InputData1.Text);
                    break;
                case "CopiedButton2":
                    // Logic for Button2
                    await Clipboard.SetTextAsync(InputData2.Text);
                    break;
                case "CopiedButton3":
                    // Logic for Button3
                    await Clipboard.SetTextAsync(InputData3.Text);
                    break;
            }
            await Task.Delay(3000);
            button.Text = "\ue798";
        }
    }

    private void FullViewForCopiedData(object sender, EventArgs e)
    {
        if (CopiedDataViewButton.Text == "\ue702")
        {
            MobileCopiedData.HeightRequest = 2 * MobileCopiedData.HeightRequest;
            CopiedDataViewButton.Text = "\ue703";
        }
        else
        {
            MobileCopiedData.HeightRequest = MobileCopiedData.HeightRequest / 2;
            CopiedDataViewButton.Text = "\ue702";
        }
    }
    private void StartBubbleAnimation()
    {
        if (!tapped)
        {
            var bubbleEffect = new Animation(v => SubmitForm.Scale = v, 1, 1.05, Easing.CubicInOut);
            var fadeEffect = new Animation(v => SubmitForm.Opacity = v, 1, 0.5, Easing.CubicInOut);

            animation.Add(0, 0.5, bubbleEffect);
            animation.Add(0, 0.5, fadeEffect);
            animation.Add(0.5, 1, new Animation(v => SubmitForm.Scale = v, 1.05, 1, Easing.CubicInOut));
            animation.Add(0.5, 1, new Animation(v => SubmitForm.Opacity = v, 1, 1, Easing.CubicInOut));

            animation.Commit(this, "BubbleEffect", length: 1500, easing: Easing.CubicInOut, repeat: () => true);

        }
    }

    private void StopBubbleAnimation()
    {
        this.AbortAnimation("BubbleEffect");
        tapped = false;
    }
}