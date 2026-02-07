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
    internal class DeepSeekResponse
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("object")] public string Object { get; set; }

        [JsonProperty("created")] public long Created { get; set; }

        [JsonProperty("model")] public string Model { get; set; }

        [JsonProperty("choices")] public List<DeepSeekChoice> Choices { get; set; }

        [JsonProperty("usage")] public DeepSeekUsage Usage { get; set; }

        [JsonProperty("system_fingerprint")] public string SystemFingerprint { get; set; }
    }

    [Serializable]
    internal class DeepSeekChoice
    {
        [JsonProperty("index")] public int Index { get; set; }

        [JsonProperty("message")] public DeepSeekMessage Message { get; set; }

        [JsonProperty("logprobs")] public DeepSeekLogProbs LogProbs { get; set; }

        [JsonProperty("finish_reason")] public string FinishReason { get; set; }
    }

    [Serializable]
    internal class DeepSeekMessage
    {
        [JsonProperty("role")] public string Role { get; set; }

        [JsonProperty("content")] public string Content { get; set; }

        [JsonProperty("reasoning_content")] public string ReasoningContent { get; set; }
    }

    [Serializable]
    internal class DeepSeekLogProbs
    {
        // Note: Since logprobs is null in your example, this class is minimal
        // You can expand it based on actual DeepSeek documentation
        [JsonProperty("tokens")] public List<string> Tokens { get; set; }

        [JsonProperty("token_logprobs")] public List<double> TokenLogProbs { get; set; }

        [JsonProperty("top_logprobs")] public List<Dictionary<string, double>> TopLogProbs { get; set; }

        [JsonProperty("text_offset")] public List<int> TextOffset { get; set; }
    }

    [Serializable]
    internal class DeepSeekUsage
    {
        [JsonProperty("prompt_tokens")] public int PromptTokens { get; set; }

        [JsonProperty("completion_tokens")] public int CompletionTokens { get; set; }

        [JsonProperty("total_tokens")] public int TotalTokens { get; set; }

        [JsonProperty("prompt_tokens_details")]
        public DeepSeekPromptTokensDetails PromptTokensDetails { get; set; }

        [JsonProperty("completion_tokens_details")]
        public DeepSeekCompletionTokensDetails CompletionTokensDetails { get; set; }

        [JsonProperty("prompt_cache_hit_tokens")]
        public int PromptCacheHitTokens { get; set; }

        [JsonProperty("prompt_cache_miss_tokens")]
        public int PromptCacheMissTokens { get; set; }

        // Helper property for reasoning tokens
        public int ReasoningTokens => CompletionTokensDetails?.ReasoningTokens ?? 0;
    }

    [Serializable]
    internal class DeepSeekPromptTokensDetails
    {
        [JsonProperty("cached_tokens")] public int CachedTokens { get; set; }
    }

    [Serializable]
    internal class DeepSeekCompletionTokensDetails
    {
        [JsonProperty("reasoning_tokens")] public int ReasoningTokens { get; set; }
    }

    [Serializable]
    internal class DeepSeekError
    {
        [JsonProperty("message")] public string Message { get; set; }

        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("param")] public string Param { get; set; }

        [JsonProperty("code")] public string Code { get; set; }
    }

    public class DeepSeekLlm : BaseLlm
    {
        public DeepSeekLlm(string apiKey, string model = "deepseek-reasoner") :
            base(apiKey, "https://api.deepseek.com/chat/completions", model)
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

            var response = JsonConvert.DeserializeObject<DeepSeekResponse>(llmGenericResponse.Response);
            var deepSeekContentResult = "";

            ClearThinkingCache();
            if (response is { Choices: { Count: > 0 } })
            {
                foreach (var choice in response.Choices)
                {
                    deepSeekContentResult += choice.Message.Content;
                    AddToThinkingCache(choice.Message.ReasoningContent);
                }
            }
            else
            {
                // throw new NoResponseException(request, request.error, request.downloadHandler.text);
                return new LlmGenericResponse(llmGenericResponse.Response, false);
            }

            return new LlmGenericResponse(deepSeekContentResult, true);
        }
    }
}