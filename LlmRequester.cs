namespace FALLA
{
    public class LlmRequester
    {
        private string _apiKey;
        private LlmType _llmType;
        private BaseLlm _llm;

        public LlmRequester(string apiKey, LlmType llmType)
        {
            _apiKey = apiKey;
            _llmType = llmType;
            _llm = LLmFactory.CreateLlm(_llmType, _apiKey);
        }

    // public void Submit()
    // {
    //     if (string.IsNullOrEmpty(inputText.text))
    //     {
    //         outputText.text = "Please enter a prompt!";
    //         return;
    //     }
    //     
    //     StartCoroutine(SendGenerateContentRequest());
    // }
    //
    // private IEnumerator SendGenerateContentRequest()
    // {
    //     if (submitButton != null) submitButton.interactable = false;
    //
    //     var url = $"{apiUrl}{model}:generateContent?key={apiKey}";
    //     
    //     Debug.Log(url);
    //     
    //     var requestData = new GeminiRequest
    //     {
    //         contents = new List<Content> {
    //             new Content {
    //                 parts = new List<Part> { new Part { text = inputText.text } }
    //             }
    //         },
    //         generationConfig = new GenerationConfig {
    //             temperature = this.temperature,
    //             topK = this.topK,
    //             topP = this.topP,
    //             maxOutputTokens = this.maxOutputTokens,
    //             stopSequences = stopSequences.Length > 0 ? new List<string>(stopSequences) : null
    //         }
    //     };
    //     // Add safety settings if enabled
    //     if (enableSafetySettings)
    //     {
    //         requestData.safetySettings = new List<SafetySetting>
    //         {
    //             new SafetySetting { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
    //             new SafetySetting { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
    //             new SafetySetting { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_MEDIUM_AND_ABOVE" },
    //             new SafetySetting { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_MEDIUM_AND_ABOVE" }
    //         };
    //     }
    //     
    //     var jsonPayload = JsonConvert.SerializeObject(requestData);
    //
    //     Debug.Log($"Sending request to: {url}");
    //     Debug.Log($"Request body: {jsonPayload}");
    //     
    //     using var request = new UnityWebRequest(url, "POST");
    //     var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
    //     request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //     request.downloadHandler = new DownloadHandlerBuffer();
    //     request.SetRequestHeader("Content-Type", "application/json");
    //     
    //     outputText.text = $"Request sent...";
    //     yield return request.SendWebRequest();
    //     try
    //     {
    //         if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
    //         {
    //             var errorResponse = JsonConvert.DeserializeObject<GeminiResponse>(request.downloadHandler.text);
    //             var errorMessage = !string.IsNullOrEmpty(errorResponse.error) 
    //                 ? errorResponse.error 
    //                 : request.error;
    //             outputText.text = $"Error: {request.error}";
    //             Debug.LogError($"API Error: {errorMessage}\nFull response: {request.downloadHandler.text}");
    //         }
    //         else
    //         {
    //             var responseText = request.downloadHandler.text;
    //             Debug.Log($"Raw response: {responseText}");
    //             var response = JsonConvert.DeserializeObject<GeminiResponse>(responseText);
    //             if (response?.candidates == null || response.candidates.Count == 0)
    //             {
    //                 outputText.text = "No response generated";
    //                 Debug.LogError("Invalid response structure");
    //             }
    //             else
    //             {
    //                 var generatedText = response.candidates[0].content.parts[0].text;
    //                 outputText.text = generatedText;
    //                 
    //                 if (response.promptFeedback?.safetyRatings == null) yield break;
    //                 foreach (var rating in response.promptFeedback.safetyRatings)
    //                 {
    //                     Debug.Log($"Safety: {rating.category} - {rating.probability}");
    //                 }
    //             }
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         outputText.text = $"Processing error: {e.Message}";
    //         Debug.LogError(e);
    //     }
    //     finally
    //     {
    //         if (submitButton != null) submitButton.interactable = true;    
    //     }
    // }
    }
}