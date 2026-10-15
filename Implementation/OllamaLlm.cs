using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace FALLA.Implementation
{
    public class OllamaLlm : BaseLlm
    {
        [Serializable]
        internal class OllamaResponse
        {
            public string id;
            public bool done;
            [JsonProperty("done_reason")] public string doneReason;
            public long created;
            public string model;
            public OllamaChoices[] choices;
            [JsonProperty("created_at")] public string createdAt;
            public OllamaMessage message;
            [JsonProperty("total_duration")] public long totalDuration;
            [JsonProperty("load_duration")] public long loadDuration;
            [JsonProperty("prompt_eval_count")] public int promptEvalCount;

            [JsonProperty("prompt_eval_cached_count")]
            public int promptEvalCachedCount;

            [JsonProperty("prompt_eval_duration")] public long promptEvalDuration;
            [JsonProperty("eval_count")] public int evalCount;
            [JsonProperty("eval_duration")] public long evalDuration;
        }

        [Serializable]
        internal class OllamaChoices
        {
            public int index;
            public OllamaMessage message;
            [JsonProperty("finish_reason")] public string finishReason;
        }

        [Serializable]
        internal class OllamaUsage
        {
            [JsonProperty("prompt_tokens")] public int promptTokens;
            [JsonProperty("completion_tokens")] public int completionTokens;
            [JsonProperty("total_tokens")] public int totalTokens;
        }

        [Serializable]
        internal class OllamaMessage
        {
            public string role;
            public string content;
        }

        public OllamaLlm(string apiKey, LlmConfig config) : base(apiKey, config)
        {
        }

        public override async Task<LlmGenericResponse> SendRequest(string content)
        {
            var requestBody = new Dictionary<string, object>
            {
                { "model", config.model },
                {
                    "messages", new[]
                    {
                        new { role = "user", content }
                    }
                },
                { "temperature", Temperature },
                { "max_tokens", MaxOutputTokens },
                { "stream", false }
            };

            var llmGenericResponse = await AttemptRequest(() =>
            {
                var request = new UnityWebRequest(apiUrl, "POST");
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer ollama");
                var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                return request;
            });

            if (!llmGenericResponse.success)
            {
                return llmGenericResponse;
            }

            var result = llmGenericResponse.response;
            var response = JsonConvert.DeserializeObject<OllamaResponse>(result);
            var messageContent = "";

            var valid = response.message != null || response.choices != null;
            if (!valid) return new LlmGenericResponse(messageContent, false);

            if (response.message != null)
            {
                messageContent = response.message.content;
            }
            else if (response.choices != null)
            {
                messageContent =
                    response.choices.Aggregate(messageContent, (current, choice) => current + choice.message.content);
            }

            return new LlmGenericResponse(messageContent, true);
        }

        public static LlmConfig GetDefaultConfig()
        {
            var defaultDeepSeek =
                new LlmConfig("", "", "http://192.168.2.50:11434/v1/chat/completions", "phi3");
            return defaultDeepSeek;
        }
    }
}