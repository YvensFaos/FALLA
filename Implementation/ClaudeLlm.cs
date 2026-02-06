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
    internal class ClaudeMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    internal class ClaudeRequest
    {
        public string model;
        [JsonProperty("max_tokens")] public int maxTokens;
        public ClaudeMessage[] messages;
    }

    [Serializable]
    internal class ClaudeContent
    {
        public string type;
        public string text;
    }

    [Serializable]
    internal class ClaudeResponse
    {
        public string id;
        public string type;
        public string role;
        public ClaudeContent[] content;
        public string model;
    }

    public class ClaudeLlm : BaseLlm
    {
        private readonly string _version;

        public ClaudeLlm(string apiKey, string model = "claude-sonnet-4-5-20250929", string version = "2023-06-01") :
            base(apiKey, "https://api.anthropic.com/v1/messages", model)
        {
            _version = version;
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
                max_tokens = MaxOutputTokens
            };

            var llmGenericResponse = await AttemptRequest(() =>
            {
                var request = new UnityWebRequest(APIUrl, "POST");
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                request.SetRequestHeader("x-api-key", APIKey);
                request.SetRequestHeader("anthropic-version", _version);
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
            var response = JsonConvert.DeserializeObject<ClaudeResponse>(result);
            if (response.content is not { Length: > 0 } && response.content[0].text != null)
            {
                return new LlmGenericResponse(result, 0, false);
            }

            var claudeMessage = response.content[0].text;
            if (claudeMessage == null)
            {
                return new LlmGenericResponse(result, 0, false);
            }

            var claudeContentResult = "";
            ClearThinkingCache();
            foreach (var claudeContent in response.content)
            {
                claudeContentResult = claudeContent.text;
            }

            return new LlmGenericResponse(claudeContentResult, 0, true);
        }
    }
}