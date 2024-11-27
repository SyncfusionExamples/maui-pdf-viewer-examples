using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using Azure;

namespace SmartFill
{
    public class AIHelper
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

        #region Constructor

        public AIHelper()
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
        /// Retrieves an answer from the GPT-4 model using the provided user prompt.
        /// </summary>
        /// <param name="userPrompt">The prompt input from the user for generating a response.</param>
        /// <returns>A string containing the response from OpenAI or an empty string if an error occurs.</returns>
        public async Task<string> GetChatCompletion(string userPrompt)
        {
            try
            {
                if (key != "OPENAI_AI_KEY" && DeploymentName != "DEPLOYMENT_NAME" && endpoint != "https://yourendpoint.com/" && Client!=null)
                {
                    ChatHistory = string.Empty;
                    ChatHistory = ChatHistory + userPrompt;
                    var response = await Client.CompleteAsync(ChatHistory);
                    return response.ToString();
                }
                else
                {
                    return " ";
                }
            }
            catch
            {
                // Return an empty string if an exception occurs
                return " ";
            }
        }
        #endregion
    }
}
