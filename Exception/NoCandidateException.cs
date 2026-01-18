using FALLA.Implementations;
using UnityEngine.Networking;

namespace FALLA.Exception
{
    public class NoCandidateException : System.Exception
    {
        private UnityWebRequest _request;
        private string _responseText;

        public NoCandidateException(UnityWebRequest request, string responseText)
        {
            _request = request;
            _responseText = responseText;
        }
    }
}