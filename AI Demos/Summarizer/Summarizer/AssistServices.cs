using OpenAI.Chat;
using Summarizer.Services;
using Syncfusion.Maui.AIAssistView;
using System.Collections.ObjectModel;
using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.AI;

namespace Summarizer
{
    /// <summary>
    /// Represents a class providing AI assistance services for text extraction, 
    /// prompt generation, and chat completion functionalities using OpenAI's GPT model.
    /// </summary>
    internal class AssistServices
    {
        #region Fields

        /// <summary>
        /// Gets or sets the extracted text from a document.
        /// </summary>
        internal string? ExtractedDocumentText { get; set; }

        /// <summary>
        /// The EndPoint
        /// </summary>
        private string endpoint = "https://yourendpoint.com/";

        /// <summary>
        /// The Deployment name
        /// </summary>
        internal string DeploymentName = "DEPLOYMENT_NAME";

        /// <summary>
        /// The AI key
        /// </summary>
        private string key = "API_KEY";

        /// <summary>
        /// The IChatClient instance
        /// </summary>
        private IChatClient? _chatClient;

        /// <summary>
        /// The chat history
        /// </summary>
        private string? _chatTranscript;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the chat history
        /// </summary>
        public string? ChatTranscript
        {
            get { return _chatTranscript; }
            set { _chatTranscript = value; }
        }

        /// <summary>
        /// Gets or sets the IChatClient instance
        /// </summary>
        public IChatClient? ChatClient
        {
            get { return _chatClient; }
            set { _chatClient = value; }
        }

        #endregion

        private readonly TextEmbeddingGenerator _embeddingGenerator = new();
        private readonly PdfSemanticIndex _semanticIndex;
        private bool _isIndexReady;
        private readonly List<(int PageNumber, string Text)> _indexedPages = new();

        internal AssistServices()
        {
            _semanticIndex = new PdfSemanticIndex(_embeddingGenerator);
            InitializeChatClient();
        }

        /// <summary>
        /// Initializes the Azure OpenAI client
        /// </summary>
        private void InitializeClient()
        {
            AzureOpenAIClient azureOpenAiClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

            ChatClient chatClient = azureOpenAiClient.GetChatClient(DeploymentName);
            _chatClient = chatClient.AsIChatClient();
        }

        private bool IsConfigured
        {
            get
            {
                return _chatClient != null
                    && !string.IsNullOrWhiteSpace(key)
                    && key != "API_KEY"
                    && !string.IsNullOrWhiteSpace(endpoint)
                    && endpoint != "https://yourendpoint.com/"
                    && DeploymentName != "DEPLOYMENT_NAME";
            }
        }

        internal async Task BuildPdfIndexAsync(IReadOnlyList<(int PageNumber, string Text)> pages, CancellationToken cancellationToken = default)
        {
            _indexedPages.Clear();
            _indexedPages.AddRange(pages);

            _embeddingGenerator.TrainOnCorpusTexts(pages.Select(page => page.Text));
            await _semanticIndex.BuildIndexFromPagesAsync(pages, cancellationToken);

            _isIndexReady = true;
        }

        private Task<List<string>> GetRelevantContextAsync(string query, int topK = 6, double minScore = 0.2, CancellationToken cancellationToken = default)
        {
            if (!_isIndexReady)
            {
                List<string> emptyContext = new List<string>();
                return Task.FromResult(emptyContext);
            }

            return _semanticIndex.GetRelevantContextChunksAsync(query, topK, 8, minScore, cancellationToken);
        }

        /// <summary>
        /// Gets a general chat completion for a prompt (non-PDF grounded).
        /// </summary>
        internal async Task<string> GetPromptAsync(string prompt)
        {
            if (!IsConfigured)
                return "Please connect OpenAI for real time queries";

            string systemPrompt = "You are a helpful assistant. Provide a clear response to the user.";
            _chatTranscript = $"System: {systemPrompt}\nUser: {prompt}";

            ChatResponse response = await _chatClient!.GetResponseAsync(_chatTranscript);
            return response.ToString();
        }

        /// <summary>
        /// Answers a question using only the indexed PDF context.
        /// </summary>
        internal async Task<string> GetDocumentGroundedAnswerAsync(string question)
        {
            if (!IsConfigured)
                return "Please connect OpenAI for real time queries";

            List<string> contextChunks = await GetRelevantContextAsync(question, topK: 6, minScore: 0.2);
            if (contextChunks==null || contextChunks.Count == 0)
                return "I couldn't index the PDF yet. Please load the document first.";

            string contextBlock = string.Join("\n\n---\n\n", contextChunks);

            string systemPrompt =
                "Answer the user's question using ONLY the provided document context.\n\n" +
                "If the question is unrelated to the document (and is not a greeting/polite message), respond politely in a <p> tag saying you cannot answer because it is not relevant to the document.\n" +
                "Keep the answer concise and derived from the document.\n" +
                "Format the answer as HTML only (no <html>/<body> wrapper).\n" +
                "All text must be the same font size. Headers should be bold but not larger than body text.\n" +
                "Put bold text inside a <p> tag, and normal content in separate <p> tags.\n\n" +
                $"DOCUMENT CONTEXT:\n{contextBlock}\n\n" +
                $"QUESTION:\n{question}";

            _chatTranscript = $"System: {systemPrompt}\nUser: {question}";

            ChatResponse response = await _chatClient!.GetResponseAsync(_chatTranscript);
            return response.ToString();
        }

        internal async Task<AssistItemSuggestion> GetSuggestionsAsync(string prompt)
        {
            AssistItemSuggestion chatSuggestions = new AssistItemSuggestion();
            ObservableCollection<ISuggestion> suggestions = new ObservableCollection<ISuggestion>();

            if (!IsConfigured || !_isIndexReady)
                return chatSuggestions;

            List<string> contextChunks = await GetRelevantContextAsync(prompt, topK: 5, minScore: 0.15);
            if (contextChunks==null ||contextChunks.Count == 0)
                return chatSuggestions;

            string contextBlock = string.Join("\n\n---\n\n", contextChunks);

            string systemPrompt =
                "You are a helpful assistant. Generate 3 short diverse questions based on the context.\n" +
                "Each question must not exceed 10 words.\n" +
                "Do not add any other content or description.\n" +
                "Output strictly as:\n1. ...\n2. ...\n3. ...\n\n" +
                $"CONTEXT:\n{contextBlock}";

            _chatTranscript = $"System: {systemPrompt}\nUser: {prompt}";

            ChatResponse response = await _chatClient!.GetResponseAsync(_chatTranscript);
            string suggestionText = response.ToString();

            string[] parts = suggestionText.Split(new[] { "1. ", "2. ", "3. " }, StringSplitOptions.RemoveEmptyEntries);

            string question1 = parts.Length > 0 ? parts[0].Trim() : string.Empty;
            string question2 = parts.Length > 1 ? parts[1].Trim() : string.Empty;
            string question3 = parts.Length > 2 ? parts[2].Trim() : string.Empty;

            if (!string.IsNullOrWhiteSpace(question1)) suggestions.Add(new AssistSuggestion { Text = question1 });
            if (!string.IsNullOrWhiteSpace(question2)) suggestions.Add(new AssistSuggestion { Text = question2 });
            if (!string.IsNullOrWhiteSpace(question3)) suggestions.Add(new AssistSuggestion { Text = question3 });

            chatSuggestions.Items = suggestions;
            return chatSuggestions;
        }
    }
}