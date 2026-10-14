using System;
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
            var requestBody = new
            {
                model = config.model,
                messages = new[]
                {
                    new { role = "user", content }
                },
                stream = false,
                temperature = Temperature,
                top_p = TopP,
                max_tokens = MaxOutputTokens
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

            ClearThinkingCache();
            return response?.message == null
                ? new LlmGenericResponse(messageContent, false)
                : new LlmGenericResponse(response.message.content, true);
        }

        public static LlmConfig GetDefaultConfig()
        {
            var defaultDeepSeek =
                new LlmConfig("", "", "http://192.168.2.50:11434/v1/chat/completions", "phi3");
            return defaultDeepSeek;
        }
    }
}