using System;
namespace FileGate.Application.Exceptions
{
    public class RecieveConnectionException : Exception
    {
        public RecieveConnectionException()
        {

        }

        public RecieveConnectionException(string message) : base(message)
        {

        }

        public RecieveConnectionException(string message, Exception inner) : base(message, inner)
        {

        } 
    }
}
