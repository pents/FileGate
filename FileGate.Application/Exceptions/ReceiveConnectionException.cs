using System;
namespace FileGate.Application.Exceptions
{
    public class ReceiveConnectionException : Exception
    {
        public ReceiveConnectionException()
        {

        }

        public ReceiveConnectionException(string message) : base(message)
        {

        }

        public ReceiveConnectionException(string message, Exception inner) : base(message, inner)
        {

        } 
    }
}
