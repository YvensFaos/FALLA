using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FALLA.Exception;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace FALLA.Implementations
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

        public GeminiLlm(string apiKey)
            : base(apiKey, "https://generativelanguage.googleapis.com/v1beta/models/", "gemini-2.5-flash-lite")
        {
            _url = $"{APIUrl}{Model}:generateContent?key={APIKey}";
        }

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
                    stopSequences = StopSequences.Length > 0 ? new List<string>(StopSequences) : null
                }
            };
            try
            {
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
            catch (System.Exception e)
            {
                //TODO change
                Debug.LogError(e);
                return null;
            }
        }
    }
}