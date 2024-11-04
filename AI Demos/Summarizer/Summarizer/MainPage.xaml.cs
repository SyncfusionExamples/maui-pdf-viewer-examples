using Syncfusion.Maui.AIAssistView;
using Syncfusion.Pdf.Parsing;

namespace Summarizer;

public partial class MainPage : ContentPage
{
    private bool tapped;
    Animation animation;
    public MainPage()
    {
        InitializeComponent();
        animation = new Animation();
        PdfViewer.DocumentLoaded += PdfViewer_DocumentLoaded;
    }

    private async void PdfViewer_DocumentLoaded(object? sender, EventArgs? e)
    {
        AssistServices AI = new AssistServices();
        if (AI.DeploymentName == "DEPLOYMENT_NAME")
            Application.Current?.MainPage?.DisplayAlert("Alert", "The Azure API key or endpoint is missing or incorrect. Please verify your credentials", "OK");
        await LoadPDFDataAsync();
        var reply = await viewModel.assistService.GetAnswerFromGPT("Read the PDF document contents and understand the concept. Provide summary for this in 3 to 4 simple sentences. Ignore about iTextSharp related points in the details");
        var suggestion = await viewModel.assistService.GetSuggestion("Provide short Summary for the document");
        AssistItem botMessage = new AssistItem() { Text = reply, Suggestion = suggestion };
        viewModel.Messages.Add(botMessage);
        viewModel.ShowLoading = false;
    }

    private void AIAssistant_Clicked(object? sender, EventArgs e)
    {
        AILayout.IsVisible = !AILayout.IsVisible;
        if (AILayout.IsVisible)
        {
            tapped = true;
            StopBubbleAnimation();
        }
        else
            StartBubbleAnimation();
    }

    internal async Task LoadPDFDataAsync()
    {
        await Task.Delay(1000);
        var documentSource = PdfViewer.DocumentSource;
        List<string> extractedText = new List<string>();
        if (documentSource != null)
        {
            Stream stream = (Stream)documentSource;
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream);
            // Loading page collections
            PdfLoadedPageCollection loadedPages = loadedDocument.Pages;
            await Task.Run(() =>
            {
                // Extract annotations to a memory stream and convert to string
                using (MemoryStream annotationStream = new MemoryStream())
                {
                    loadedDocument.ExportAnnotations(annotationStream, AnnotationDataFormat.Json);
                    string annotations = ConvertToString(annotationStream);
                    if (!String.IsNullOrEmpty(annotations))
                        extractedText.Add("Annotations: " + annotations);
                }

                // Extract form fields to a memory stream and convert to string
                using (MemoryStream formStream = new MemoryStream())
                {
                    if (loadedDocument.Form != null)
                    {
                        loadedDocument.Form.ExportData(formStream, DataFormat.Json, "form");
                        string formFields = ConvertToString(formStream);
                        if (!String.IsNullOrEmpty(formFields))
                            extractedText.Add("Form fields: " + formFields);
                    }
                }

                // Extract text from existing PDF document pages
                for (int i = 0; i < loadedPages.Count; i++)
                {
                    string text = $"... Page {i + 1} ...\n";
                    text += loadedPages[i].ExtractText();
                    extractedText.Add(text);
                }
                string result = string.Join(Environment.NewLine, extractedText);
                viewModel.assistService.ExtractedText = result;
            });
        }
    }

    private string ConvertToString(MemoryStream memoryStream)
    {
        // Reset the position of the MemoryStream to the beginning
        memoryStream.Position = 0;
        var reader = new StreamReader(memoryStream, System.Text.Encoding.UTF8);
        return reader.ReadToEnd();
    }
    private void PreviousPage(object? sender, EventArgs e)
    {
        PdfViewer.GoToPreviousPage();
        if (PdfViewer.PageNumber == 1)
        {
            goToPreviousPageButton.IsEnabled = false;
            goToNextPageButton.IsEnabled = true;
        }
        else
        {
            goToPreviousPageButton.IsEnabled = true;
            goToNextPageButton.IsEnabled = true;
        }
    }
    private void NextPage(object? sender, EventArgs e)
    {
        PdfViewer.GoToNextPage();
        if (PdfViewer.PageNumber == PdfViewer.PageCount)
        {
            goToNextPageButton.IsEnabled = false;
            goToPreviousPageButton.IsEnabled = true;
        }
        else
        {
            goToNextPageButton.IsEnabled = true;
            goToPreviousPageButton.IsEnabled = true;
        }
    }

    private void pageNumberEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        int pageNumber;
        bool check = int.TryParse(e.NewTextValue, out pageNumber);
        if (pageNumber != 0)
            PdfViewer.GoToPage(pageNumber);

    }

    private void pageNumberEntry_Focused(object sender, FocusEventArgs e)
    {
        if (!string.IsNullOrEmpty(pageNumberEntry.Text))
        {
            // Select all text in the Editor
            pageNumberEntry.CursorPosition = 0;
            pageNumberEntry.SelectionLength = pageNumberEntry.Text.Length;
        }
    }

    private void image_Loaded(object sender, EventArgs e)
    {
        StartBubbleAnimation();
    }
    private void StartBubbleAnimation()
    {
        if (!tapped)
        {
            var bubbleEffect = new Animation(v => AIButton.Scale = v, 1, 1.15, Easing.CubicInOut);
            var fadeEffect = new Animation(v => AIButton.Opacity = v, 1, 0.5, Easing.CubicInOut);
            animation.Add(0, 0.5, bubbleEffect);
            animation.Add(0, 0.5, fadeEffect);
            animation.Add(0.5, 1, new Animation(v => AIButton.Scale = v, 1.15, 1, Easing.CubicInOut));
            animation.Add(0.5, 1, new Animation(v => AIButton.Opacity = v, 0.5, 1, Easing.CubicInOut));
            animation.Commit(this, "BubbleEffect", length: 1500, easing: Easing.CubicInOut, repeat: () => true);

        }
    }

    private void StopBubbleAnimation()
    {
        this.AbortAnimation("BubbleEffect");
        tapped = false;
    }
}