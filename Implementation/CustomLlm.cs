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
    internal class CustomGenericResponse
    {
        public string model;
        public string createdAt;
        public CustomGenericMessage message;
    }
    
    [Serializable]
    internal class CustomGenericMessage
    {
        public string role;
        public string content;
        public bool done;
        [JsonProperty("done_reason")]
        public string doneReason;
        [JsonProperty("total_duration")]
        public long totalDuration;
        [JsonProperty("prompt_eval_count")]
        public int promptEvalCount;
        [JsonProperty("prompt_eval_cached_count")]
        public int promptEvalCachedCount;
        [JsonProperty("prompt_eval_duration")]
        public int promptEvalDuration;
        [JsonProperty("eval_count")]
        public int evalCount;
        [JsonProperty("eval_duration")]
        public long evalDuration;
    }
    
    public class CustomLlm : BaseLlm
    {
        public CustomLlm(string apiUrl, string model="phi3", float temperature = 0.2f, int topK = 40, int topP = 1, int maxOutputTokens = 4096) 
            : base("", apiUrl, model, temperature, topK, topP, maxOutputTokens)
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
                stream = false,
                think = "low",
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

            if (!llmGenericResponse.Success)
            {
                return llmGenericResponse;
            }

            var result = llmGenericResponse.Response;
            var response = JsonConvert.DeserializeObject<CustomGenericResponse>(result);
            if (response.message is { content: not { Length: > 0 } })
            {
                return new LlmGenericResponse(result, false);
            }
            var contentResult = response.message.content;
            return new LlmGenericResponse(contentResult, true);
        }
    }
}