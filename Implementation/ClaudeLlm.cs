using System;
using System.Threading.Tasks;
using FALLA.Exception;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace FALLA.Implementation
{
    [Serializable]
    public class ClaudeMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class ClaudeRequest
    {
        public string model;
        [JsonProperty("max_tokens")] public int maxTokens;
        public ClaudeMessage[] messages;
    }

    [Serializable]
    public class ClaudeContent
    {
        public string type;
        public string text;
    }

    [Serializable]
    public class ClaudeResponse
    {
        public string id;
        public string type;
        public string role;
        public ClaudeContent[] content;
        public string model;
    }

    public class ClaudeLlm : BaseLlm
    {
        private string _version;

        public ClaudeLlm(string apiKey, string model = "claude-sonnet-4-5-20250929", string version = "2023-06-01") :
            base(apiKey, "https://api.anthropic.com/v1/messages", model)
        {
            _version = version;
        }

        public override async Task<string> SendRequest(string content)
        {
            var requestBody = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "user", content = content }
                },
                temperature = Temperature,
                max_tokens = MaxOutputTokens
            };

            var jsonBody = JsonConvert.SerializeObject(requestBody);
            using var request = new UnityWebRequest(APIUrl, "POST");
            request.SetRequestHeader("x-api-key", APIKey);
            request.SetRequestHeader("anthropic-version", _version);
            request.SetRequestHeader("Content-Type", "application/json");
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new NoResponseException(request, request.error, request.downloadHandler.text);
            }

            var responseText = request.downloadHandler.text;

            var response = JsonConvert.DeserializeObject<ClaudeResponse>(responseText);
            if (response.content is not { Length: > 0 } && response.content[0].text != null)
            {
                throw new NoCandidateException(request, responseText);
            }

            var claudeMessage = response.content[0].text;
            if (claudeMessage == null)
            {
                throw new NoCandidateException(request, responseText);
            }

            var claudeContentResult = "";
            ClearThinkingCache();
            foreach (var claudeContent in response.content)
            {
                claudeContentResult = claudeContent.text;
            }

            return claudeContentResult;
        }
    }
}