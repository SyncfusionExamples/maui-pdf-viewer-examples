using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace TextSearchAndNavigation
{
    public partial class MainPage : ContentPage
    {
        // Stores the search results to enable navigation and clearing.
        TextSearchResult? SearchResult;

        public MainPage()
        {
            InitializeComponent();

            // Load the PDF document from embedded resources.
            Stream? stream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("TextSearchAndNavigation.Assets.PDF_Succinctly.pdf");

            if (stream != null)
                PdfViewer.LoadDocument(stream);

            // Customize the highlight colors for search results.
            PdfViewer.TextSearchSettings.CurrentMatchHighlightColor = Color.FromRgba("#8F00FF43"); // Highlight color for the current match.
            PdfViewer.TextSearchSettings.OtherMatchesHighlightColor = Color.FromRgba("#00FF0043");  // Highlight color for other matches.
        }

        // Initiates a text search when the search button is clicked.
        private void SearchButtonClicked(object sender, EventArgs e)
        {
            SearchText("the");
        }

        // Searches for the specified text asynchronously and stores the result.
        async void SearchText(string text)
        {
            PdfViewer.TextSearchProgress += PdfTextSearchProgress;
            SearchResult = await PdfViewer.SearchTextAsync(text);
        }

        private void PdfTextSearchProgress(object? sender, TextSearchProgressEventArgs? e)
        {
            if (e != null)
                // Cancel the search operation when it is running but does not need to be continued.
                e.SearchResult?.Clear();
        }

        // Moves to the next match when the next button is clicked.
        private void NextMatchButtonClicked(object sender, EventArgs e)
        {
            SearchResult?.GoToNextMatch();
        }

        // Moves to the previous match when the previous button is clicked.
        private void PreviousMatchButtonClicked(object sender, EventArgs e)
        {
            SearchResult?.GoToPreviousMatch();
        }

        // Clears the search highlights and resets the search state.
        private void CloseSearchButtonClicked(object sender, EventArgs e)
        {
            SearchResult?.Clear();
        }
    }

}
