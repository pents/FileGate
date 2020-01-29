using System;
namespace FileGate.Application.Exceptions
{
    public class RecieveMessageException : Exception
    {
        public RecieveMessageException()
        {

        }

        public RecieveMessageException(string message) : base(message)
        {

        }

        public RecieveMessageException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
