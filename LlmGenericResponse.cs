namespace FALLA
{
    public struct LlmGenericResponse
    {
        public string response;
        public int attempts;
        public bool success;

        public LlmGenericResponse(string response, int attempts, bool success)
        {
            this.response = response;
            this.attempts = attempts;
            this.success = success;
        }

        public static LlmGenericResponse EmptyResponse() => new LlmGenericResponse("", -1, false);
    }
}