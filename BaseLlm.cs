using System;
using System.Threading.Tasks;

namespace FALLA
{
    public abstract class BaseLlm
    {
        protected string APIKey;
        protected string APIUrl;
        protected string Model;
        protected float Temperature;
        protected int TopK;
        protected int TopP;
        protected int MaxOutputTokens;
        protected string[] StopSequences;

        protected BaseLlm(string apiKey, string apiUrl, string model, float temperature = 0.2f, int topK = 40, int topP = 1, int maxOutputTokens = 2048)
        {
            APIKey = apiKey;
            APIUrl = apiUrl;
            Model = model;
            Temperature = temperature;
            TopK = topK;
            TopP = topP;
            MaxOutputTokens = maxOutputTokens;
            StopSequences = Array.Empty<string>();
        }

        public abstract Task<string> SendRequest(string content);
    }
}