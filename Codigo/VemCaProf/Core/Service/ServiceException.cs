namespace Core.Service
{
    // Torna a classe ServiceException pública para ser acessível fora do assembly
    public class ServiceException : Exception
    {
        public ServiceException() { }

        public ServiceException(string message) : base(message) { }

        public ServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}