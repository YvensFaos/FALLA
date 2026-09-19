/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System;

namespace FALLA
{
    [Serializable]
    public class LlmConfig
    {
        public string apiKeyIdentifier;
        public string apiKeyFile;
        public string apiUrl;
        public string model;
        public bool think = false;
        public float temperature = 0.2f;
        public int topK = 40;
        public int topP = 1;
        public int maxOutputTokens = 4096;
        public bool local = false;
        public string version;

        public LlmConfig(string apiKeyIdentifier, string apiKeyFile, string apiUrl, string model)
        {
            this.apiKeyIdentifier = apiKeyIdentifier;
            this.apiKeyFile = apiKeyFile;
            this.apiUrl = apiUrl;
            this.model = model;
        }

        private LlmConfig()
        { }

        public LlmConfig Clone() => new()
        {
            apiKeyIdentifier = apiKeyIdentifier,
            apiKeyFile = apiKeyFile,
            apiUrl = apiUrl,
            model = model,
            think = think,
            temperature = temperature,
            topK = topK,
            topP = topP,
            maxOutputTokens = maxOutputTokens,
            local = local,
            version = version
        };
    }
}