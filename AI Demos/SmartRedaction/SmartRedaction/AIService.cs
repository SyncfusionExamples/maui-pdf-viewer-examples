using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI;
using Azure;

namespace SmartRedaction
{
    public class AIService
    {
        #region Fields

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
        private string key = "OPENAI_AI_KEY";

        /// <summary>
        /// The IChatClient instance
        /// </summary>
        private IChatClient client;

        /// <summary>
        /// The chat history
        /// </summary>
        private string chatHistory;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the chat history
        /// </summary>
        public string ChatHistory
        {
            get { return chatHistory; }
            set { chatHistory = value; }
        }

        /// <summary>
        /// Gets or sets the IChatClient instance
        /// </summary>
        public IChatClient Client
        {
            get { return client; }
            set { client = value; }
        }

        #endregion

        #region Constructor

        public AIService()
        {
            InitializeClient();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the Azure OpenAI client
        /// </summary>
        private void InitializeClient()
        {
            client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key)).AsChatClient(modelId: DeploymentName);
        }

        /// <summary>
        /// Retrieves an answer from the GPT model using the provided system prompt and extracted text.
        /// </summary>
        /// <param name="systemPrompt">The system instruction for GPT.</param>
        /// <param name="extractedText">The input text extracted from a source.</param>
        /// <returns>A string containing the response from OpenAI or an empty string if an error occurs.</returns>
        public async Task<string> GetAnswerFromGPT(string systemPrompt, string extractedText)
        {
            try
            {
                if (Client != null && key != "OPENAI_API_KEY")
                {
                    ChatHistory = string.Empty;
                    ChatHistory += $"System: {systemPrompt}\nUser: {extractedText}";
                    var response = await Client.CompleteAsync(ChatHistory);
                    return response.ToString();
                }
                else
                    return string.Empty;
            }
            catch
            {
                // Return an empty string if an exception occurs
                return string.Empty;
            }
        }

        #endregion
    }
}
