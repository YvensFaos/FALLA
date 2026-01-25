namespace FALLA.Exception
{
    public class RequestException : System.Exception
    {
        private System.Exception _originalException;
        private string _message;

        public RequestException(System.Exception originalException, string message)
        {
            _originalException = originalException;
            _message = message;
        }
    }
}