using UnityEngine.Networking;

namespace FALLA.Exception
{
    public class NoResponseException : System.Exception
    {
        private UnityWebRequest _request;
        private string _error;
        private string _downloadHandlerText;

        public NoResponseException(UnityWebRequest request, string error, string downloadHandlerText)
        {
            _request = request;
            _error = error;
            _downloadHandlerText = downloadHandlerText;
        }
    }
}