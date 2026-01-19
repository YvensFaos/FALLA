/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 * 
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */


namespace FALLA.Exception
{
    public class LlmKeyNotFoundException : System.Exception
    {
        private readonly string _filePath;
        private readonly LlmType _llmType;
        private readonly string _llmKey;

        public LlmKeyNotFoundException(string filePath, LlmType llmType, string llmKey)
        {
            _filePath = filePath;
            _llmType = llmType;
            _llmKey = llmKey;
        }
        
        public override string Message => $"Error: LLM Key Not Found for {_llmType}. File Path: {_filePath}. LLM Key: {_llmKey}.";
    }
}