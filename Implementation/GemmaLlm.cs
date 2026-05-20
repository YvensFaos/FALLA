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
    internal class GemmaResponse
    {
        public List<GemmaCandidate> candidates;
        public GemmaUsageMetadata usageMetadata;
        public string modelVersion;
        public string responseId;
        public string error;   // in case of API error
    }

    [Serializable]
    internal class GemmaCandidate
    {
        public GemmaContent content;
        public string finishReason;
        public int index;
    }

    [Serializable]
    internal class GemmaContent
    {
        public List<GemmaPart> parts;
        public string role;   // "model" or "user"
    }

    [Serializable]
    internal class GemmaPart
    {
        public string text;
        public bool? thought;   // true for internal reasoning, false/null for final output
    }

    [Serializable]
    internal class GemmaUsageMetadata
    {
        public int promptTokenCount;
        public int candidatesTokenCount;
        public int totalTokenCount;
        public List<GemmaTokensDetails> promptTokensDetails;
        public int? thoughtsTokenCount;
        public string serviceTier;
    }

    [Serializable]
    internal class GemmaTokensDetails
    {
        public string modality;
        public int tokenCount;
    }
    
    public class GemmaLlm : BaseLlm
    {
        private readonly string _url;
        
        public GemmaLlm(string apiKey, string model = "gemma-4-26b-a4b-it")
            : base(apiKey, "https://generativelanguage.googleapis.com/v1beta/models/", model)
        {
            _url = $"{apiUrl}{Model}:generateContent?key={this.apiKey}";
        }
        
        /// <summary>
        /// Send the content request to Gemma.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public override async Task<LlmGenericResponse> SendRequest(string content)
        {
            var requestData = new GeminiRequest
            {
                contents = new List<GeminiContent>
                {
                    new()
                    {
                        parts = new List<GeminiPart> { new() { text = content } }
                    }
                },
                generationConfig = new GeminiGenerationConfig
                {
                    temperature = Temperature,
                    topK = TopK,
                    topP = TopP,
                    maxOutputTokens = MaxOutputTokens,
                    stopSequences = StopSequences.Count > 0 ? StopSequences : null
                }
            };

            var llmGenericResponse = await AttemptRequest(() =>
            {
                var request = new UnityWebRequest(_url, "POST");
                var jsonPayload = JsonConvert.SerializeObject(requestData);
                var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                return request;
            });

            if (!llmGenericResponse.Success)
            {
                return llmGenericResponse;
            }

            var response = JsonConvert.DeserializeObject<GemmaResponse>(llmGenericResponse.Response);
            if (response?.candidates == null || response.candidates.Count == 0)
            {
                return new LlmGenericResponse(llmGenericResponse.Response, false);
            }

            if (response.candidates == null || response.candidates.Count == 0 || response.candidates[0]?.content?.parts == null)
            {
                return llmGenericResponse;
            }
            
            var parts = response.candidates[0].content.parts;
            var generatedText = parts.Find(part => part.thought == null).text;
            return new LlmGenericResponse(generatedText, true);
        }
    }
}