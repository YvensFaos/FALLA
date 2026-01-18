using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace FALLA
{
    public class LlmCallerObject : MonoBehaviour
    {
        [Header("API Configuration")] [SerializeField]
        private string apiKey = "YOUR_API_KEY";

        [Header("Llm Settings")] [SerializeField]
        private LlmType llmType;

        private BaseLlm _llm;
        private bool _ready;
        private string _response;

        private void Start()
        {
            _llm = LLmFactory.CreateLlm(LlmType.Gemini, apiKey);
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
            }
        }

        public bool IsReady() => _ready;
        public string GetResponse() => _response;
    }
}