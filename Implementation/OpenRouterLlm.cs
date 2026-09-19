/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace FALLA.Implementation
{
    [Serializable]
    internal class OpenRouterResponse
    {
        [JsonProperty("choices")] public List<OpenRouterChoice> Choices { get; set; }
    }

    [Serializable]
    internal class OpenRouterChoice
    {
        [JsonProperty("message")] public OpenRouterMessage Message { get; set; }
    }

    [Serializable]
    internal class OpenRouterMessage
    {
        [JsonProperty("role")] public string Role { get; set; }

        [JsonProperty("content")] public string Content { get; set; }
    }

    public class OpenRouterLlm : BaseLlm
    {
        public OpenRouterLlm(string apiKey, LlmConfig config) : base(apiKey, config)
        {
            var defaultConfig = GetDefaultConfig();
            if (string.IsNullOrEmpty(config.apiUrl))
            {
                apiUrl = defaultConfig.apiUrl;
            }

            if (string.IsNullOrEmpty(config.model))
            {
                Model = defaultConfig.model;
            }
        }

        public override async Task<LlmGenericResponse> SendRequest(string content)
        {
            var requestBody = new Dictionary<string, object>
            {
                { "model", Model },
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
            if (StopSequences.Count > 0)
            {
                requestBody.Add("stop", StopSequences);
            }

            var llmGenericResponse = await AttemptRequest(() =>
            {
                var request = new UnityWebRequest(apiUrl, "POST");
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                if (!IsLocal())
                {
                    request.SetRequestHeader("Authorization", "Bearer " + apiKey);    
                }
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

            var response = JsonConvert.DeserializeObject<OpenRouterResponse>(llmGenericResponse.response);
            var openRouterContentResult = "";

            ClearThinkingCache();
            if (response is not { Choices: { Count: > 0 } })
            {
                return new LlmGenericResponse(llmGenericResponse.response, false);
            }

            foreach (var choice in response.Choices)
            {
                if (!string.IsNullOrEmpty(choice.Message?.Content))
                {
                    openRouterContentResult += choice.Message.Content;
                }
            }

            return string.IsNullOrEmpty(openRouterContentResult)
                ? new LlmGenericResponse(llmGenericResponse.response, false)
                : new LlmGenericResponse(openRouterContentResult, true);
        }

        public static LlmConfig GetDefaultConfig()
        {
            var defaultOpenRouter =
                new LlmConfig("", "",
                    "https://openrouter.ai/api/v1/chat/completions",
                    "qwen/qwen3.6-35b-a3b");
            return defaultOpenRouter;
        }
    }
}