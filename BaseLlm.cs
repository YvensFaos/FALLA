/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FALLA
{
    public abstract class BaseLlm
    {
        protected readonly string APIKey;
        protected readonly string APIUrl;

        public string Model { get; set; }
        protected float Temperature { get; set; }
        protected int TopK { get; set; }
        protected int TopP { get; }
        protected int MaxOutputTokens { get; }
        protected List<string> StopSequences { get; }

        protected BaseLlm(string apiKey, string apiUrl, string model, float temperature = 0.2f, int topK = 40,
            int topP = 1, int maxOutputTokens = 2048)
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

        public abstract Task<string> SendRequest(string content);

        public void AddStopSequence(string sequence)
        {
            StopSequences.Add(sequence);
        }
    }
}