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
    public class MistralResponse
    {
        public string id { get; set; }
        public long created { get; set; }
        public string model { get; set; }
        [JsonProperty("usage")] public MistralUsage MistralUsage { get; set; }
        [JsonProperty("object")] public string @object { get; set; }
        public List<MistralChoice> choices { get; set; }
    }

    [Serializable]
    public class MistralUsage
    {
        [JsonProperty("prompt_tokens")] public int PromptTokens { get; set; }

        [JsonProperty("total_tokens")] public int TotalTokens { get; set; }

        [JsonProperty("completion_tokens")] public int CompletionTokens { get; set; }
    }

    [Serializable]
    public class MistralChoice
    {
        public int index { get; set; }
        [JsonProperty("finish_reason")] public string FinishReason { get; set; }

        [JsonProperty("message")] public MistralMessage Message { get; set; }
    }

    [Serializable]
    public class MistralMessage
    {
        public string role { get; set; }

        [JsonProperty("tool_calls")] public object ToolCalls { get; set; }

        public List<MistralContent> content { get; set; }
    }

    [Serializable]
    public class MistralContent
    {
        public string type { get; set; }

        [JsonProperty("text")] public string Text { get; set; }

        public List<MistralThinkingItem> thinking { get; set; }
    }

    [Serializable]
    public class MistralThinkingItem
    {
        public string type { get; set; }
        public string text { get; set; }
    }

    public class MistralLlm : BaseLlm
    {
        public MistralLlm(string apiKey, string model = "magistral-small-2509") :
            base(apiKey, "https://api.mistral.ai/v1/chat/completions", model)
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
                max_tokens = MaxOutputTokens
            };

            var result = await AttemptRequest(() =>
            {
                var request = new UnityWebRequest(APIUrl, "POST");
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                request.SetRequestHeader("Authorization", "Bearer " + APIKey);
                request.SetRequestHeader("Content-Type", "application/json");
                var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                return request;
            });

            var response = JsonConvert.DeserializeObject<MistralResponse>(result);
            if (response.choices is not { Count: > 0 } && response.choices[0].Message != null)
            {
                // throw new NoCandidateException(request, responseText);
                return null;
            }

            var mistralMessage = response.choices[0].Message;
            if (mistralMessage == null)
            {
                // throw new NoCandidateException(request, responseText);
                return null;
            }

            var mistralContentResult = "";
            ClearThinkingCache();
            foreach (var mistralContent in mistralMessage.content)
            {
                switch (mistralContent.type)
                {
                    case "text":
                        mistralContentResult = mistralContent.Text;
                        break;
                    case "thinking" when mistralContent.thinking != null:
                    {
                        foreach (var thinkingItem in mistralContent.thinking)
                        {
                            AddToThinkingCache(thinkingItem.text);
                        }

                        break;
                    }
                }
            }

            return mistralContentResult;
        }
    }
}