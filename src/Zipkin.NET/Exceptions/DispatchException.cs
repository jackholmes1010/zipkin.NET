using System;

namespace Zipkin.NET.Exceptions
{
    public class DispatchException : Exception
    {
        public DispatchException(string message, Exception innerException)
        : base (message, innerException)
        {
        }
    }
}
