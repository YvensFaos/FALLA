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
        [Header("API Configuration")] 
        [SerializeField]
        private string apiKeyFile;

        [Header("Llm Settings")] 
        [SerializeField]
        private LlmType llmType;
        [SerializeField] 
        private List<LlmTypeKeyPair> llmTypeKeyPairs;

        private BaseLlm _llm;
        private bool _ready;
        private string _response;

        private void Start()
        {
            var llmKeyPair = llmTypeKeyPairs.Find((pair) => pair.type == llmType);
            var keyValue = JsonFileReader.GetValueFromValuePairJson(apiKeyFile, llmKeyPair.key);
            if (string.IsNullOrEmpty(keyValue))
            {
                throw new LlmKeyNotFoundException(apiKeyFile, llmType, llmKeyPair.key);
            }
            _llm = LLmFactory.CreateLlm(llmType, keyValue);
            _ready = false;
            _response = "";
        }

        public void CallLlm(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return;
            }

            _ready = false;
            _response = "";
            SubmitAsync(prompt);
        }

        public void CallLlmWithCallback(string prompt, UnityAction<string> callback)
        {
            CallLlm(prompt);
            StartCoroutine(CallLlmCoroutine(callback));
        }

        private IEnumerator CallLlmCoroutine(UnityAction<string> callback)
        {
            yield return new WaitUntil(IsReady);
            callback(GetResponse());
        }

        private async void SubmitAsync(string prompt)
        {
            try
            {
                _response = await _llm.SendRequest(prompt);
                _ready = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(e.Message);
            }
        }

        public bool IsReady() => _ready;
        public string GetResponse() => _response;
    }
}