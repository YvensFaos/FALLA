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
using FALLA.Exception;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace FALLA.Implementation
{
    [Serializable]
    internal class GeminiRequest
    {
        public List<GeminiContent> contents;
        public GeminiGenerationConfig generationConfig;
        public List<GeminiSafetySetting> safetySettings;
    }

    [Serializable]
    internal class GeminiContent
    {
        public List<GeminiPart> parts;
    }

    [Serializable]
    public class GeminiPart
    {
        public string text;
    }

    [Serializable]
    internal class GeminiResponse
    {
        public List<GeminiCandidate> candidates;
        public GeminiPromptFeedback promptFeedback;
        public string error;
    }

    [Serializable]
    internal class GeminiCandidate
    {
        public GeminiContent content;
    }

    [Serializable]
    internal class GeminiGenerationConfig
    {
        public float temperature;
        public int topP;
        public int topK;
        public int maxOutputTokens;
        public List<string> stopSequences;
    }

    [Serializable]
    internal class GeminiPromptFeedback
    {
        public List<GeminiSafetyRating> safetyRatings;
    }

    [Serializable]
    internal class GeminiSafetyRating
    {
        public string category;
        public string probability;
    }

    [Serializable]
    internal class GeminiSafetySetting
    {
        public string category;
        public string threshold;
    }

    public class GeminiLlm : BaseLlm
    {
        private readonly string _url;

        public GeminiLlm(string apiKey, string model = "gemini-2.5-flash-lite")
            : base(apiKey, "https://generativelanguage.googleapis.com/v1beta/models/", model)
        {
            _url = $"{APIUrl}{Model}:generateContent?key={APIKey}";
        }

        /// <summary>
        /// Send the content request to Gemini.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="NoResponseException"></exception>
        /// <exception cref="NoCandidateException"></exception>
        public override async Task<string> SendRequest(string content)
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

            var jsonPayload = JsonConvert.SerializeObject(requestData);
            using var request = new UnityWebRequest(_url, "POST");
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new NoResponseException(request, request.error, request.downloadHandler.text);
            }

            var responseText = request.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<GeminiResponse>(responseText);
            if (response?.candidates == null || response.candidates.Count == 0)
            {
                throw new NoCandidateException(request, responseText);
            }

            var generatedText = response.candidates[0].content.parts[0].text;
            return generatedText;
        }
    }
}