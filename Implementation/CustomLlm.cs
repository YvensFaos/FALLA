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
using UnityEngine;
using UnityEngine.Networking;

namespace FALLA.Implementation
{
    [Serializable]
    internal class CustomGenericResponse
    {
        //{"model":"phi3","created_at":"2026-09-18T19:03:35.563836Z","message": ... 
        public string model;
        public string createdAt;
        public CustomGenericMessage message;
    }
    
    [Serializable]
    internal class CustomGenericMessage
    {
        //{"role":"assistant","content":"Hello, Ground Control. Yes, I am here, Major Tom. I am powered by Microsoft's GPT-4, an advanced language prediction model designed to understand and generate human-like text."},"done":true,"done_reason":"stop","total_duration":672546583,"load_duration":6968500,"prompt_eval_count":29,"prompt_eval_cached_count":28,"prompt_eval_duration":33128000,"eval_count":44,"eval_duration":629167000} 
        public string role;
        public string content;
        public bool done;
        public string doneReason;
        public int totalDuration;
        public int promptEvalCount;
        public int promptEvalCachedCount;
        public int promptEvalDuration;
        public int evalCount;
        public int evalDuration;
        
        
    }
    
    public class CustomLlm : BaseLlm
    {
        public CustomLlm(string apiUrl, string model="phi3", float temperature = 0.2f, int topK = 40, int topP = 1, int maxOutputTokens = 4096) 
            : base("", apiUrl, model, temperature, topK, topP, maxOutputTokens)
        {
        }

        public override async Task<LlmGenericResponse> SendRequest(string content)
        {
            /*
             *
             curl -s http://192.168.2.50:11434/api/chat -d '{
                 "model": "phi3",
                 "messages": [
                   {"role": "user", "content": "Ground Control to Major Tom!"}
                 ],
                 "stream": false
               }'
             */
            var requestBody = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "user", content }
                },
                stream = false,
                //temperature = Temperature,
                //max_tokens = MaxOutputTokens
            };

            var llmGenericResponse = await AttemptRequest(() =>
            {
                var request = new UnityWebRequest(apiUrl, "POST");
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                //request.SetRequestHeader("x-api-key", apiKey);
                //request.SetRequestHeader("anthropic-version", _version);
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

            Debug.Log(llmGenericResponse.Response);
            var result = llmGenericResponse.Response;
            var response = JsonConvert.DeserializeObject<CustomGenericResponse>(result);
            if (response.message.content is not { Length: > 0 } && response.message != null)
            {
                return new LlmGenericResponse(result, false);
            }
            var contentResult = response.message.content;
            return new LlmGenericResponse(contentResult, true);
        }
    }
}