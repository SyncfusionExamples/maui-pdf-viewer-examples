using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace CustomSearchToolbar
{
    public partial class MainPage : ContentPage
    {
        Stream? m_pdfDocumentStream;
        TextSearchResult? SearchResult;
        public MainPage()
        {
            InitializeComponent();

            // Load PDF document from resources
            m_pdfDocumentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("CustomSearchToolbar.zero_degree_rotated_document.pdf");
            PdfViewer.LoadDocument(m_pdfDocumentStream);
        }

        //Event handler for the search button click, initiates a text search in a PDF viewer.
        private async void Search_Clicked(object sender, EventArgs e)
        {
            // Set search options based on user preferences.
            TextSearchOptions options =  SetSearchOptions();

            // Perform an asynchronous text search in the PDF viewer.
            SearchResult = await PdfViewer.SearchTextAsync(TextField.Text, options);

            if(SearchResult != null)
            {
                // Display an alert message based on the outcome.
                if (SearchResult.TotalMatchesCount == 0)
                    await DisplayAlert("No Match Found!", "Sorry, no matches found. Please try different keywords", "Cancel");
                else
                {
                    // Update UI to show the total matches count and the current match index.
                    MatchesCount.IsVisible = true;
                    MatchesCount.Text = "Matches count: " + SearchResult.TotalMatchesCount.ToString();
                    CurrentMatch.IsVisible = true;
                    CurrentMatch.Text = "Current Match: " + SearchResult.CurrentMatchIndex.ToString();
                }
            }
        }

        // Set search options based on user preferences.
        private TextSearchOptions SetSearchOptions()
        {
            TextSearchOptions options = new TextSearchOptions();
            options = TextSearchOptions.None;
            if (MatchCase.IsChecked)
                options = TextSearchOptions.CaseSensitive;
            return options;
        }

        // Moves to the previous match in the search results.
        private void Previous_Clicked(object sender, EventArgs e)
        {
            SearchResult?.GoToPreviousMatch();
            CurrentMatch.IsVisible = true;
            CurrentMatch.Text = "Current Match: " + SearchResult?.CurrentMatchIndex.ToString();
        }

        //Moves to the next match in the search results.
        private void Next_Clicked(object sender, EventArgs e)
        {
            SearchResult?.GoToNextMatch();
            CurrentMatch.IsVisible = true;
            CurrentMatch.Text = "Current Match: " + SearchResult?.CurrentMatchIndex.ToString();
        }

        //Changes settings of the highlight colors for the current and other matches in the PDF viewer.
        private void ChangeHightlightColor_Clicked(object sender, EventArgs e)
        {
            PdfViewer.TextSearchSettings.CurrentMatchHighlightColor = Color.FromRgba("#8F00FF43");
            PdfViewer.TextSearchSettings.OtherMatchesHighlightColor = Color.FromRgba("#00FF0043");
        }

        //Clears the search results and updates UI elements after closing the search.
        private void Close_Clicked(object sender, EventArgs e)
        {
            SearchResult?.Clear();
            MatchesCount.Text = "Matches count: " + SearchResult?.TotalMatchesCount.ToString();
            CurrentMatch.Text = "Current Match: " + SearchResult?.CurrentMatchIndex.ToString();
        }
    }

}
