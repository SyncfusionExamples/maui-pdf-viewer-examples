using Syncfusion.Maui.AIAssistView;
using System.Collections.ObjectModel;
using Azure.AI.OpenAI;
using Azure;

namespace Summarizer
{
    /// <summary>
    /// Represents a class providing AI assistance services for text extraction, 
    /// prompt generation, and chat completion functionalities using OpenAI's GPT model.
    /// </summary>
    internal class AssistServices
    {

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
        internal string deploymentName = "DEPLOYMENT_NAME";

        /// <summary>
        /// The AI key
        /// </summary>
        private string key = "AZURE_OPENAI_API_KEY";

        /// <summary>
        /// The AzureOpenAI client
        /// </summary>
        internal OpenAIClient? client;

        /// <summary>
        /// The ChatCompletion option
        /// </summary>
        private ChatCompletionsOptions? chatCompletions;

        internal AssistServices()
        {
            chatCompletions = new ChatCompletionsOptions
            {
                DeploymentName = deploymentName,
                Temperature = (float)1.2f,
                NucleusSamplingFactor = (float)0.9,
                FrequencyPenalty = 0.8f,
                PresencePenalty = 0.8f
            };
            client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
        }

        /// <summary>
        /// Generates a static prompt message.
        /// </summary>
        /// <param name="prompt">The input prompt string.</param>
        /// <returns>A predefined message requesting OpenAI connection for real-time queries.</returns>
        internal async Task<string> GetPrompt(string prompt)
        {
            if (client != null && chatCompletions != null && key!= "AZURE_OPENAI_API_KEY")
            {
                chatCompletions.Messages.Clear();
                chatCompletions.Messages.Add(new ChatRequestSystemMessage("Please provide the prompt for responce" + prompt));
                chatCompletions.Messages.Add(new ChatRequestUserMessage(prompt));
                var response = await client.GetChatCompletionsAsync(chatCompletions);
                return response.Value.Choices[0].Message.Content;
            }
            else
            {
                return "Please connect OpenAI for real time queries";
            }
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
                if (ExtractedText != null && client != null && chatCompletions != null && key != "AZURE_OPENAI_API_KEY")
                {
                    string message = ExtractedText;
                    var systemPrompt = "You are a helpful assistant. Use the provided PDF document pages and pick a precise page to answer the user question,Ignore about iTextSharp related points in the details, Strictly don't bold any text all text need to plain text. Pages: " + message;
                    chatCompletions.Messages.Clear();
                    chatCompletions.Messages.Add(new ChatRequestSystemMessage(systemPrompt));
                    chatCompletions.Messages.Add(new ChatRequestUserMessage(question));
                    var response = await client.GetChatCompletionsAsync(chatCompletions);
                    return response.Value.Choices[0].Message.Content;
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
                if (ExtractedText != null && chatCompletions != null && client != null && key != "AZURE_OPENAI_API_KEY")
                {
                    chatCompletions.Messages.Clear();
                    chatCompletions.Messages.Add(new ChatRequestSystemMessage(systemPrompt));
                    chatCompletions.Messages.Add(new ChatRequestUserMessage(ExtractedText));
                    var response = await client.GetChatCompletionsAsync(chatCompletions);
                    return response.Value.Choices[0].Message.Content;
                }
                else
                {
                    return "Please connect OpenAI for real time queries";
                }
            }
            catch
            {
                return "Please connect OpenAI for real time queries";
            }
        }
    }
}