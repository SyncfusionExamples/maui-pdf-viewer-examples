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
        internal string? ExtractedText { get; set; }

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
        private IChatClient? client;

        /// <summary>
        /// The chat history
        /// </summary>
        private string? chatHistory;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the chat history
        /// </summary>
        public string? ChatHistory
        {
            get { return chatHistory; }
            set { chatHistory = value; }
        }

        /// <summary>
        /// Gets or sets the IChatClient instance
        /// </summary>
        public IChatClient? Client
        {
            get { return client; }
            set { client = value; }
        }

        #endregion

        internal AssistServices()
        {
            InitializeClient();
        }

        /// <summary>
        /// Initializes the Azure OpenAI client
        /// </summary>
        private void InitializeClient()
        {
            client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key)).AsChatClient(modelId: DeploymentName);
        }

        /// <summary>
        /// Generates a static prompt message.
        /// </summary>
        /// <param name="prompt">The input prompt string.</param>
        /// <returns>A predefined message requesting OpenAI connection for real-time queries.</returns>
        internal async Task<string> GetPrompt(string prompt)
        {
            if (this.Client != null && key != "OPENAI_AI_KEY")
            {
                ChatHistory = string.Empty;
                ChatHistory += $"System: {"Please provide the prompt for responce" + prompt}\nUser: {prompt}";
                var response = await Client.CompleteAsync(ChatHistory);
                return response.ToString();
            }
            else
                return "Please connect OpenAI for real time queries";
        }

        /// <summary>
        /// Gets a solution to a given prompt by using either local embeddings or extracted text,
        /// depending on the platform.
        /// </summary>
        /// <param name="question">The user's question to be processed.</param>
        /// <returns>A task representing the asynchronous operation, with a solution string as the result.</returns>
        internal async Task<string> GetSolutionToPrompt(string question)
        {
            try
            {
                // Use extracted text
                if (this.ExtractedText != null && this.Client != null && key != "OPENAI_AI_KEY")
                {
                    string message = ExtractedText;
                    var systemPrompt = "Read the PDF document contents, understand the concept, and select the precise page to answer the user's question. Ignore any points related to iTextSharp. Ensure that all text is plain and not bolded. Pages: Question: " + question;
                    ChatHistory = string.Empty;
                    ChatHistory += $"System: {systemPrompt}\nUser: {ExtractedText}";
                    var response = await Client.CompleteAsync(ChatHistory);
                    return response.ToString();
                }
                return "Please connect OpenAI for real time queries";
            }
            catch
            {
                return "Please connect OpenAI for real time queries";
            }
        }

        /// <summary>
        /// Generates suggestions based on a given prompt.
        /// </summary>
        /// <param name="prompt">The input prompt string.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AssistItemSuggestion"/> object as the result.</returns>
        internal async Task<AssistItemSuggestion> GetSuggestion(string prompt)
        {
            var chatSuggestions = new AssistItemSuggestion();
            var suggestions = new ObservableCollection<ISuggestion>();
            var suggestion = await GetAnswerFromGPT("You are a helpful assistant. Your task is to analyze the provided text and generate 3 short diverse questions and each question should not exceed 10 words.");
            if (suggestion != "Please connect OpenAI for real time queries")
            {
                string[] parts = suggestion.Split(new string[] { "1. ", "2. ", "3. " }, StringSplitOptions.RemoveEmptyEntries);
                // Store the parts in separate variables
                string question1 = parts.Length > 0 ? parts[0].Trim() : string.Empty;
                string question2 = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                string question3 = parts.Length > 2 ? parts[2].Trim() : string.Empty;
                suggestions.Add(new AssistSuggestion() { Text = question1 });
                suggestions.Add(new AssistSuggestion() { Text = question2 });
                suggestions.Add(new AssistSuggestion() { Text = question3 });
                chatSuggestions.Items = suggestions;
            }
            return chatSuggestions;
        }

        /// <summary>
        /// Gets an answer from the GPT model using only a system prompt.
        /// </summary>
        /// <param name="systemPrompt">The system prompt to guide the AI.</param>
        /// <returns>A task representing the asynchronous operation, with the answer string as the result.</returns>
        internal async Task<string> GetAnswerFromGPT(string systemPrompt)
        {
            try
            {
                if (this.ExtractedText != null &&  this.Client != null && key != "OPENAI_AI_KEY")
                {
                    ChatHistory = string.Empty;
                    ChatHistory += $"System: {systemPrompt}\nUser: {ExtractedText}";
                    var response = await Client.CompleteAsync(ChatHistory);
                    return response.ToString();
                }
                else
                    return "Please connect OpenAI for real time queries";
            }
            catch
            {
                return "Please connect OpenAI for real time queries";
            }
        }
    }
}