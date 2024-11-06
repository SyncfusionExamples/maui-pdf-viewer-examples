using Azure;
using Azure.AI.OpenAI;

namespace SmartRedaction
{
    public class AIService
    {
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
        private string key = "AZURE_OPENAI_API_KEY";

        /// <summary>
        /// The AzureOpenAI client
        /// </summary>
        private OpenAIClient? client;

        /// <summary>
        /// The ChatCompletion option
        /// </summary>
        internal ChatCompletionsOptions? chatCompletions;

        public AIService()
        {
            chatCompletions = new ChatCompletionsOptions
            {
                DeploymentName = this.DeploymentName,
                Temperature = (float)1.2f,
                NucleusSamplingFactor = (float)0.9,
                FrequencyPenalty = 0.8f,
                PresencePenalty = 0.8f
            };
            client = new OpenAIClient(new Uri(this.endpoint), new AzureKeyCredential(this.key));
        }
        public async Task<string> GetAnswerFromGPT(string systemPrompt, string extractedtext)
        {
            try
            {
                string message = extractedtext;
                if (this.client != null && this.chatCompletions != null && this.key != "AZURE_OPENAI_API_KEY")
                {
                    chatCompletions.Messages.Clear();
                    chatCompletions.Messages.Add(new ChatRequestSystemMessage(systemPrompt));
                    chatCompletions.Messages.Add(new ChatRequestUserMessage(message));
                    var response = await client.GetChatCompletionsAsync(chatCompletions);
                    return response.Value.Choices[0].Message.Content;
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}
