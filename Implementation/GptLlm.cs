/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        [JsonProperty("object")] public string chatObject;
        public GptChoice[] choices;
        [JsonProperty("service_tier")] public string serviceTier;
        [JsonProperty("system_fingerprint")] public string systemFingerPrint;
        public GptUsage usage;

        [JsonProperty("completion_tokens_details")]
        public GptCompletionTokenDetails completionTokenDetails;
    }

    [System.Serializable]
    internal class GptCompletionTokenDetails
    {
        [JsonProperty("reasoning_tokens")] public int reasoningTokens;
        [JsonProperty("audio_tokens")] public int audioTokens;

        [JsonProperty("accepted_prediction_tokens")]
        public int acceptedPredictionTokens;

        [JsonProperty("rejected_prediction_tokens")]
        public int rejectedPredictionTokens;
    }

    [System.Serializable]
    internal class GptUsage
    {
        [JsonProperty("prompt_tokens")] public int promptTokens;
        [JsonProperty("completion_tokens")] public int completionTokens;
        [JsonProperty("total_tokens")] public int totalTokens;

        [JsonProperty("prompt_tokens_details")]
        public GptPromptTokenDetails promptTokenDetails;
    }

    [System.Serializable]
    internal class GptPromptTokenDetails
    {
        [JsonProperty("cached_tokens")] public int cachedTokens;
        [JsonProperty("audio_tokens")] public int audioTokens;
    }

    [System.Serializable]
    internal class GptChoice
    {
        public int index;
        [JsonProperty("finish_reason")] public string finishReason;
        public GptMessage message;
    }

    public class GptLlm : BaseLlm
    {
        public GptLlm(string apiKey, string model = "gpt-4.1-mini") :
            base(apiKey, "https://api.openai.com/v1/chat/completions", model)
        {
        }

        public override async Task<LlmGenericResponse> SendRequest(string content)
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

            var llmGenericResponse = await AttemptRequest(() =>
            {
                var request = new UnityWebRequest(apiUrl, "POST");
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                request.SetRequestHeader("Authorization", "Bearer " + apiKey);
                request.SetRequestHeader("Content-Type", "application/json");
                var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                return request;
            });

            if (!llmGenericResponse.Success)
            {
                return llmGenericResponse;
            }

            var response = JsonConvert.DeserializeObject<GptResponse>(llmGenericResponse.Response);
            var gptContentResult = "";

            ClearThinkingCache();
            if (response is { choices: { Length: > 0 } })
            {
                gptContentResult = response.choices.Aggregate(gptContentResult,
                    (current, choice) => current + choice.message.content);
            }
            else
            {
                return new LlmGenericResponse(llmGenericResponse.Response, false);
            }

            return new LlmGenericResponse(gptContentResult, true);
        }
    }
}