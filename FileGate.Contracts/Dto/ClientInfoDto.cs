﻿using System;
namespace FileGate.Contracts.Dto
{
    public class ClientInfoDto : MessageBase
    {
        public ClientInfoDto()
        {
            Type = Enums.MessageType.CONNECT;
        }

        public Guid ClientId { get; set; }
    }
}
