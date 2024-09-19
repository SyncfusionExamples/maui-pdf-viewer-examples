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
        internal string deploymentName = "DEPLOYMENT_NAME";

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
                DeploymentName = deploymentName,
                Temperature = (float)1.2f,
                NucleusSamplingFactor = (float)0.9,
                FrequencyPenalty = 0.8f,
                PresencePenalty = 0.8f
            };
            client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

        }
        public async Task<string> GetAnswerFromGPT(string systemPrompt,string extractedtext)
        {
            try
            {
                string message = extractedtext;
                if (client != null && chatCompletions != null && key!= "AZURE_OPENAI_API_KEY")
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
