using System;
namespace FileGate.Application.Exceptions
{
    public class SendMessageException : Exception
    {
        public SendMessageException()
        {

        }

        public SendMessageException(string message) : base(message)
        {

        }

        public SendMessageException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
