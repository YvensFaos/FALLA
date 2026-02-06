/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace FALLA
{
    public abstract class BaseLlm
    {
        protected readonly string APIKey;
        protected readonly string APIUrl;

        protected delegate UnityWebRequest WebRequestDelegate();

        public string Model { get; set; }
        protected float Temperature { get; set; }
        protected int TopK { get; set; }
        protected int TopP { get; }
        protected int MaxOutputTokens { get; }
        protected List<string> StopSequences { get; }

        private List<string> _thinkingCache;

        protected BaseLlm(string apiKey, string apiUrl, string model, float temperature = 0.2f, int topK = 40,
            int topP = 1, int maxOutputTokens = 4096)
        {
            APIKey = apiKey;
            APIUrl = apiUrl;
            Model = model;
            Temperature = temperature;
            TopK = topK;
            TopP = topP;
            MaxOutputTokens = maxOutputTokens;
            StopSequences = new List<string>();
        }

        public abstract Task<LlmGenericResponse> SendRequest(string content);

        protected async Task<LlmGenericResponse> AttemptRequest(WebRequestDelegate webRequestDelegate)
        {
            var request = webRequestDelegate.Invoke();
            await request.SendWebRequest();
            var response = request.result == UnityWebRequest.Result.Success
                ? new LlmGenericResponse(request.downloadHandler.text, 0, true)
                : new LlmGenericResponse(request.error, 0, false);
            request.Dispose();
            return response;
        }

        public void AddStopSequence(string sequence)
        {
            StopSequences.Add(sequence);
        }

        protected void ClearThinkingCache()
        {
            _thinkingCache = new List<string>();
        }

        protected void AddToThinkingCache(string sequence)
        {
            _thinkingCache.Add(sequence);
        }

        /// <summary>
        /// Returns the last cached thinking messages compiled from the last request.
        /// </summary>
        /// <returns></returns>
        public List<string> GetThinkingCache()
        {
            return _thinkingCache;
        }
    }
}