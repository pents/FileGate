using System;
namespace FileGate.Contracts.Dto
{
    public class ClientInfoDto : MessageBase
    {
        public ClientInfoDto()
        {
            Type = Enums.MessageType.Connect;
        }

        public Guid ClientId { get; set; }
    }
}
