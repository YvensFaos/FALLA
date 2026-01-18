using System;
using FALLA.Implementations;

namespace FALLA
{
    public static class LLmFactory
    {
        public static BaseLlm CreateLlm(LlmType type, string apiKey)
        {
            return type switch
            {
                LlmType.Gemini => new GeminiLlm(apiKey),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}