/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace FALLA
{
    public abstract class LlmGenericCallerObject : MonoBehaviour
    {
        protected BaseLlm llm;
        private bool _ready;
        protected bool initialized;
        private LlmGenericResponse _response;

        protected void InitializeFlags()
        {
            _ready = false;
            _response = LlmGenericResponse.EmptyResponse();
            initialized = true;
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

        // ReSharper disable once AsyncVoidMethod
        private async void SubmitAsync(string prompt)
        {
            _response = await llm.SendRequest(prompt);
            _ready = true;
        }

        public bool IsReady() => _ready;
        public LlmGenericResponse GetResponse() => _response;
    }
}