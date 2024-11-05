using Azure.AI.OpenAI;
using Azure;

namespace SmartFill
{
    public class AIHelper 
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
        private string apiKey = "AZURE_OPENAI_API_KEY";

        /// <summary>
        /// The AzureOpenAI client
        /// </summary>
        // C#
        internal OpenAIClient? openAIClient;

        /// <summary>
        /// The ChatCompletion option
        /// </summary>
        private ChatCompletionsOptions? chatCompletionsOptions;

        public AIHelper()
        {
            chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = deploymentName,
                Temperature = (float)1.2f,
                NucleusSamplingFactor = (float)0.9,
                FrequencyPenalty = 0.8f,
                PresencePenalty = 0.8f
            };
            openAIClient = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
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
                if (apiKey != "AZURE_OPENAI_API_KEY" && deploymentName != "DEPLOYMENT_NAME" && endpoint != "https://yourendpoint.com/")
                {
                    chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(userPrompt));
                    var response = await openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
                    return response.Value.Choices[0].Message.Content;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please provide your Azure OpenAI API key, deployment name, and endpoint in the AIHelper class.", "OK");
                    return string.Empty;
                }
            }
            catch (Exception exception)
            {
                await Application.Current.MainPage.DisplayAlert("Error", exception.Message, "OK");
                return string.Empty;
            }
        }

    }
}