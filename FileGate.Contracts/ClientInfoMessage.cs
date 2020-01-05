using System;
namespace FileGate.Contracts
{
    public class ClientInfoMessage : MessageBase
    {
        public ClientInfoMessage()
        {
            Type = Enums.MessageType.Connect;
        }

        public Guid ClientId { get; set; }
    }
}
