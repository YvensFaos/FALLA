namespace FALLA
{
    public struct LlmGenericResponse
    {
        public readonly string Response;
        public readonly bool Success;

        public LlmGenericResponse(string response, bool success)
        {
            Response = response;
            Success = success;
        }

        public static LlmGenericResponse EmptyResponse() => new LlmGenericResponse("", false);
    }
}