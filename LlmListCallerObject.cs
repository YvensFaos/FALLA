/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System.Collections.Generic;
using FALLA.Exception;
using FALLA.Helper;
using UnityEngine;

namespace FALLA
{
    public class LlmListCallerObject : LlmGenericCallerObject
    {
        [SerializeField] private LlmType selectedLlmType;
        [SerializeField] private string selectedModel;
        [SerializeField] private List<LlmTypeKeyPair> llmTypeKeyPairs;

        private void Awake()
        {
            if (initialized) return;
            LoadModel(selectedLlmType, selectedModel);
            selectedModel = llm.Model;
            InitializeFlags();
        }

        public void LoadModel(LlmType llmType, string model)
        {
            //Find the pair in the list
            var llmTypeKeyPair = llmTypeKeyPairs.Find(pair => pair.type == llmType);

            //Reads the apiKey for the pair
            var llmConfig = llmTypeKeyPair.config;
            var configAPIKeyFile = llmConfig.apiKeyFile;
            var configAPIKeyIdentifier = llmConfig.apiKeyIdentifier;
            var keyValue = llmConfig.local
                ? configAPIKeyFile
                : JsonFileReader.GetValueFromValuePairJson(configAPIKeyFile,
                    configAPIKeyIdentifier);

            var copyConfig = llmConfig.Clone();
            if (!string.IsNullOrEmpty(model))
            {
                copyConfig.model = model;
            }

            if (!llmConfig.local && string.IsNullOrEmpty(keyValue))
            {
                throw new LlmKeyNotFoundException(configAPIKeyFile, llmType,
                    configAPIKeyIdentifier);
            }

            selectedLlmType = llmType;
            selectedModel = model;
            llm = LLmFactory.CreateLlm(llmType, keyValue, copyConfig);
        }


        [ContextMenu("Reload Model")]
        public void ReloadModel()
        {
            LoadModel(selectedLlmType, selectedModel);
        }

        public string GetLlmModel() => selectedModel;

        public override string ToString()
        {
            return $"API-{base.ToString()}|{selectedModel}";
        }
    }
}