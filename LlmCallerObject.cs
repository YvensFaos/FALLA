/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System.Collections;
using System.Collections.Generic;
using FALLA.Exception;
using FALLA.Helper;
using UnityEngine;
using UnityEngine.Events;

namespace FALLA
{
    public class LlmCallerObject : MonoBehaviour
    {
        [Header("API Configuration")] [SerializeField]
        private string apiKeyFile;

        [Header("Llm Settings")] [SerializeField]
        private LlmType llmType;

        [SerializeField] private string llmModel;
        [SerializeField] private List<LlmTypeKeyPair> llmTypeKeyPairs;

        private BaseLlm _llm;
        private bool _ready;
        private LlmGenericResponse _response;

        private void Awake()
        {
            LoadModel(llmModel);
            llmModel = _llm.Model;

            _ready = false;
            _response = LlmGenericResponse.EmptyResponse();
        }

        [ContextMenu("Reload Model")]
        public void ReloadModel()
        {
            LoadModel(llmModel);
        }

        public void LoadModel(string newModel)
        {
            var llmKeyPair = llmTypeKeyPairs.Find((pair) => pair.type == llmType);
            var keyValue = JsonFileReader.GetValueFromValuePairJson(apiKeyFile, llmKeyPair.key);
            if (string.IsNullOrEmpty(keyValue))
            {
                throw new LlmKeyNotFoundException(apiKeyFile, llmType, llmKeyPair.key);
            }

            llmModel = newModel;
            _llm = string.IsNullOrEmpty(llmModel)
                ? LLmFactory.CreateLlm(llmType, keyValue)
                : LLmFactory.CreateLlm(llmType, keyValue, llmModel);
        }

        public void CallLlm(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return;
            }

            _ready = false;
            _response = LlmGenericResponse.EmptyResponse();
            SubmitAsync(prompt);
        }

        public void CallLlmWithCallback(string prompt, UnityAction<LlmGenericResponse> callback)
        {
            CallLlm(prompt);
            StartCoroutine(CallLlmCoroutine(callback));
        }

        private IEnumerator CallLlmCoroutine(UnityAction<LlmGenericResponse> callback)
        {
            yield return new WaitUntil(IsReady);
            callback(GetResponse());
        }

        private async void SubmitAsync(string prompt)
        {
            _response = await _llm.SendRequest(prompt);
            _ready = true;
        }

        public bool IsReady() => _ready;
        public LlmGenericResponse GetResponse() => _response;
        public LlmType GetLlmType() => llmType;
        public string GetLlmModel() => llmModel;

        public override string ToString()
        {
            return $"{llmType}|{llmModel}";
        }
    }
}