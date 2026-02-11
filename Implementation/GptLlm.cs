/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace FALLA.Implementation
{
    [System.Serializable]
    internal class GptMessage
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    internal class GptResponse
    {
        public string id;
        public GptResponseOutput[] output;
    }
    
    [System.Serializable]
    internal class GptResponseOutput
    {
        public GptResponseContent[] content;
    }

    [System.Serializable]
    internal class GptResponseContent
    {
        public string type;
        public string text;
    }

    public class GptLlm : BaseLlm
    {
        public GptLlm(string apiKey, string model = "gpt-4.1-mini") :
            base(apiKey, "https://api.openai.com/v1/responses", model)
        {
        }

        public override async Task<LlmGenericResponse> SendRequest(string content)
        {
            var requestBody = new
            {
                model = Model,
                input = new[]
                {
                    new { role = "user", content = content }
                },
                temperature = Temperature,
                max_output_tokens = MaxOutputTokens
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

            var response = JsonConvert.DeserializeObject<GptResponse>(llmGenericResponse.Response);
            var gptContentResult = "";

            ClearThinkingCache();
            if (response?.output == null) return new LlmGenericResponse(gptContentResult, false);
            foreach (var item in response.output)
            {
                if (item.content == null) continue;
                foreach (var contentItem in item.content)
                {
                    if (contentItem.type == "output_text")
                        gptContentResult += contentItem.text;
                }
            }

            return new LlmGenericResponse(gptContentResult, true);
        }
    }
}