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
        if (viewModel.assistService.DeploymentName == "DEPLOYMENT_NAME")
        {
            await Application.Current?.Windows?.FirstOrDefault()?.Page?.DisplayAlertAsync("Alert","The Azure API key or endpoint is missing or incorrect. Please verify your credentials","OK")!;
        }

        await LoadPDFDataAsync();
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
        var documentSource = PdfViewer.DocumentSource;
        if (documentSource == null) return;

        var pages = new List<(int PageNumber, string Text)>();

        var stream = (Stream)documentSource;
        if (stream.CanSeek) stream.Position = 0;

        await Task.Run(() =>
        {
            using var loadedDocument = new PdfLoadedDocument(stream);
            var loadedPages = loadedDocument.Pages;

            for (int i = 0; i < loadedPages.Count; i++)
            {
                var pageNumber = i + 1;
                var text = loadedPages[i].ExtractText() ?? string.Empty;
                pages.Add((pageNumber, $"... Page {pageNumber} ...\n{text}"));
            }
        });

        await viewModel.assistService.BuildPdfIndexAsync(pages);
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