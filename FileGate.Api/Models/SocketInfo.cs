using System;
using System.Net.WebSockets;

namespace FileGate.Api.Models
{
    public class SocketInfo
    {
        public SocketInfo(WebSocket socket)
        {
            Id = Guid.NewGuid();
            SocketConnection = socket;
        }

        public Guid Id { get; set; }
        public WebSocket SocketConnection { get; set; }
    }
}
