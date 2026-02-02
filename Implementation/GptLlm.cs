using System.Linq;
using System.Threading.Tasks;
using FALLA.Exception;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace FALLA.Implementation
{
    [System.Serializable]
    internal class GptMessage
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    internal class GptResponse
    {
        public string id;
        [JsonProperty("object")]
        public string chatObject;
        public GptChoice[] choices;
        [JsonProperty("service_tier")]
        public string serviceTier;
        [JsonProperty("system_fingerprint")]
        public string systemFingerPrint;
        public GptUsage usage;
        [JsonProperty("completion_tokens_details")]
        public GptCompletionTokenDetails completionTokenDetails;
    }

    [System.Serializable]
    internal class GptCompletionTokenDetails
    {
        [JsonProperty("reasoning_tokens")]
        public int reasoningTokens;
        [JsonProperty("audio_tokens")]
        public int audioTokens;
        [JsonProperty("accepted_prediction_tokens")]
        public int acceptedPredictionTokens;
        [JsonProperty("rejected_prediction_tokens")]
        public int rejectedPredictionTokens;
    }

    [System.Serializable]
    internal class GptUsage
    {
        [JsonProperty("prompt_tokens")]
        public int promptTokens;
        [JsonProperty("completion_tokens")]
        public int completionTokens;
        [JsonProperty("total_tokens")]
        public int totalTokens;
        [JsonProperty("prompt_tokens_details")]
        public GptPromptTokenDetails promptTokenDetails;
    }

    [System.Serializable]
    internal class GptPromptTokenDetails
    {
        [JsonProperty("cached_tokens")]
        public int cachedTokens;
        [JsonProperty("audio_tokens")]
        public int audioTokens;
    }

    [System.Serializable]
    internal class GptChoice
    {
        public int index;
        [JsonProperty("finish_reason")]
        public string finishReason;
        public GptMessage message;
    }

    public class GptLlm : BaseLlm
    {
        public GptLlm(string apiKey, string model = "gpt-4.1-mini") :
            base(apiKey, "https://api.openai.com/v1/chat/completions", model)
        {
        }

        public override async Task<string> SendRequest(string content)
        {
            var requestBody = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "user", content }
                },
                temperature = Temperature,
                max_tokens = MaxOutputTokens,
                stream = false
            };

            var jsonBody = JsonConvert.SerializeObject(requestBody);
            using var request = new UnityWebRequest(APIUrl, "POST");
            request.SetRequestHeader("Authorization", "Bearer " + APIKey);
            request.SetRequestHeader("Content-Type", "application/json");

            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new NoResponseException(request, request.error, request.downloadHandler.text);
            }

            var responseText = request.downloadHandler.text;

            var response = JsonConvert.DeserializeObject<GptResponse>(responseText);
            var gptContentResult = "";

            ClearThinkingCache();
            if (response is { choices: { Length: > 0 } })
            {
                gptContentResult = response.choices.Aggregate(gptContentResult, (current, choice) => current + choice.message.content);
            }
            else
            {
                throw new NoResponseException(request, request.error, request.downloadHandler.text);
            }

            return gptContentResult;
        }
    }
}