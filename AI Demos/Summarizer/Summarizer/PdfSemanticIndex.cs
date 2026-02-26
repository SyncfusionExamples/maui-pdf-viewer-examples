using Microsoft.Extensions.AI;
using System.Text.RegularExpressions;

namespace Summarizer.Services;

internal class PdfSemanticIndex
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _pageEmbeddingGenerator;
    private readonly List<IndexedPdfPage> _indexedPdfPages = new List<IndexedPdfPage>();

    private record IndexedPdfPage(int PageNumber, string PageText, float[] EmbeddingVector);

    public PdfSemanticIndex(IEmbeddingGenerator<string, Embedding<float>> pageEmbeddingGenerator)
    {
        _pageEmbeddingGenerator = pageEmbeddingGenerator;
    }

    public int Count => _indexedPdfPages.Count;

    public async Task BuildIndexFromPagesAsync(
        IReadOnlyList<(int PageNumber, string Text)> pages,
        CancellationToken cancellationToken = default)
    {
        _indexedPdfPages.Clear();

        if (_pageEmbeddingGenerator is TextEmbeddingGenerator textEmbeddingGenerator)
        {
            IEnumerable<string> corpusTexts = pages.Select(page => page.Text);
            textEmbeddingGenerator.TrainOnCorpusTexts(corpusTexts);
        }

        IEnumerable<string> pageTexts = pages.Select(page => page.Text);

        GeneratedEmbeddings<Embedding<float>> generatedPageEmbeddings =
            await _pageEmbeddingGenerator.GenerateAsync(pageTexts, cancellationToken: cancellationToken);

        for (int pageIndex = 0; pageIndex < pages.Count; pageIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            float[] pageEmbeddingVector = generatedPageEmbeddings[pageIndex].Vector.ToArray();
            (int PageNumber, string Text) page = pages[pageIndex];

            _indexedPdfPages.Add(new IndexedPdfPage(page.PageNumber, page.Text, pageEmbeddingVector));
        }
    }

    public async Task<List<string>> GetRelevantContextChunksAsync(string queryText, int topKCandidates = 20, int maxPagesToReturn = 8, double minimumCosineSimilarity = 0.2, CancellationToken cancellationToken = default)
    {
        if (_indexedPdfPages.Count == 0)
        {
            return new List<string>();
        }

        string[] queryTexts = new string[] { queryText };

        GeneratedEmbeddings<Embedding<float>> generatedQueryEmbeddings =
            await _pageEmbeddingGenerator.GenerateAsync(queryTexts, cancellationToken: cancellationToken);

        float[] queryEmbeddingVector = generatedQueryEmbeddings[0].Vector.ToArray();

        List<(double SimilarityScore, IndexedPdfPage Page)> scoredPages =
            new List<(double SimilarityScore, IndexedPdfPage Page)>(_indexedPdfPages.Count);

        foreach (IndexedPdfPage indexedPdfPage in _indexedPdfPages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            double cosineSimilarity = ComputeCosineSimilarity(queryEmbeddingVector, indexedPdfPage.EmbeddingVector);
            if (cosineSimilarity >= minimumCosineSimilarity)
            {
                scoredPages.Add((cosineSimilarity, indexedPdfPage));
            }
        }

        List<IndexedPdfPage> selectedPages = scoredPages
            .OrderByDescending(item => item.SimilarityScore)
            .Take(topKCandidates)
            .Take(maxPagesToReturn)
            .Select(item => item.Page)
            .ToList();

        if (selectedPages.Count == 0)
        {
            return new List<string>();
        }

        IEnumerable<IndexedPdfPage> selectedPagesInDocumentOrder = selectedPages.OrderBy(page => page.PageNumber);
        return BuildContextChunksFromPages(selectedPagesInDocumentOrder);
    }

    private static List<string> BuildContextChunksFromPages(IEnumerable<IndexedPdfPage> pages)
    {
        const int maxCharactersPerPage = 1200;
        const bool includePageHeader = true;

        List<string> contextChunks = new List<string>();

        foreach (IndexedPdfPage page in pages)
        {
            string normalizedPageText = NormalizePdfText(page.PageText);

            if (normalizedPageText.Length > maxCharactersPerPage)
            {
                normalizedPageText = normalizedPageText[..maxCharactersPerPage];
            }

            string contextChunk = includePageHeader
                ? $"[page-{page.PageNumber}]\n{normalizedPageText}"
                : normalizedPageText;

            contextChunks.Add(contextChunk);
        }

        return contextChunks;
    }

    private static string NormalizePdfText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        // Join hyphenated line breaks: "exam-\nple" -> "example"
        string textWithJoinedHyphenation = Regex.Replace(text, @"(\w)-\s*\r?\n\s*(\w)", "$1$2");

        string normalizedLineEndings = textWithJoinedHyphenation.Replace("\r\n", "\n");

        IEnumerable<string> trimmedNonTrivialLines = normalizedLineEndings.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 2);

        string compactSingleLineText = string.Join(' ', trimmedNonTrivialLines);
        compactSingleLineText = Regex.Replace(compactSingleLineText, @"\s{2,}", " ").Trim();

        return compactSingleLineText;
    }

    private static double ComputeCosineSimilarity(float[] vectorA, float[] vectorB)
    {
        int commonLength = Math.Min(vectorA.Length, vectorB.Length);

        double dotProduct = 0;
        double sumSquaresA = 0;
        double sumSquaresB = 0;

        for (int i = 0; i < commonLength; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            sumSquaresA += vectorA[i] * vectorA[i];
            sumSquaresB += vectorB[i] * vectorB[i];
        }

        if (sumSquaresA <= 0 || sumSquaresB <= 0)
        {
            return 0;
        }

        return dotProduct / (Math.Sqrt(sumSquaresA) * Math.Sqrt(sumSquaresB));
    }
}