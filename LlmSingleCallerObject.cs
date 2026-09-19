/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using FALLA.Exception;
using FALLA.Helper;
using UnityEngine;

namespace FALLA
{
    public class LlmSingleCallerObject : LlmGenericCallerObject
    {
        [SerializeField] private LlmType llmType;
        [SerializeField] private string model;
        [SerializeField] private LlmConfig config;

        private void Awake()
        {
            if (initialized) return;
            LoadModel();
            InitializeFlags();
        }

        private void LoadModel()
        {
            var configAPIKeyFile = config.apiKeyFile;
            var configAPIKeyIdentifier = config.apiKeyIdentifier;
            var keyValue = config.local
                ? configAPIKeyFile
                : JsonFileReader.GetValueFromValuePairJson(configAPIKeyFile,
                    configAPIKeyIdentifier);

            if (!config.local && string.IsNullOrEmpty(keyValue))
            {
                throw new LlmKeyNotFoundException(configAPIKeyFile, llmType,
                    configAPIKeyIdentifier);
            }

            llm = LLmFactory.CreateLlm(llmType, keyValue, config);
        }

        public LlmType GetLlmType() => llmType;
        public string GetModel() => model;
        public LlmConfig GetConfig() => config;
        public void SetConfig(LlmConfig newConfig) => config = newConfig;
    }
}