/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace FALLA.Implementation
{
    [Serializable]
    internal class PhiResponse
    {
        public string model;
        public string createdAt;
        public PhiMessage message;
    }

    [Serializable]
    internal class PhiMessage
    {
        public string role;
        public string content;
        public bool done;
        [JsonProperty("done_reason")] public string doneReason;
        [JsonProperty("total_duration")] public long totalDuration;
        [JsonProperty("prompt_eval_count")] public int promptEvalCount;

        [JsonProperty("prompt_eval_cached_count")]
        public int promptEvalCachedCount;

        [JsonProperty("prompt_eval_duration")] public int promptEvalDuration;
        [JsonProperty("eval_count")] public int evalCount;
        [JsonProperty("eval_duration")] public long evalDuration;
    }

    public class PhiLlm : BaseLlm
    {
        public PhiLlm(string apiKey, LlmConfig config) : base(apiKey, config)
        {
            var defaultConfig = GetDefaultConfig();
            if (string.IsNullOrEmpty(config.model))
            {
                Model = defaultConfig.model;
            }
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
                stream = false,
                options = new
                {
                    temperature = Temperature,
                    top_k = TopK,
                    top_p = TopP,
                    num_predict = MaxOutputTokens,
                }
            };

            var llmGenericResponse = await AttemptRequest(() =>
            {
                var request = new UnityWebRequest(apiUrl, "POST");
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                request.SetRequestHeader("Content-Type", "application/json");
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
            var response = JsonConvert.DeserializeObject<PhiResponse>(result);
            if (response.message is { content: not { Length: > 0 } })
            {
                return new LlmGenericResponse(result, false);
            }

            var contentResult = response.message.content;
            return new LlmGenericResponse(contentResult, true);
        }

        public static LlmConfig GetDefaultConfig()
        {
            var defaultDeepSeek =
                new LlmConfig("", "", "", "phi3");
            return defaultDeepSeek;
        }
    }
}