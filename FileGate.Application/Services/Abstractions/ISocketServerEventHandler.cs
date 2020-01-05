using System;
using Fleck;

namespace FileGate.Application.Services.Abstractions
{
    public interface ISocketServerEventHandler
    {
        IWebSocketConnection GetSocket(Guid clientId);
        void OnMessage(string message, IWebSocketConnection socket);
        void OnError(Exception ex);
        void OnOpen(IWebSocketConnection socket);
        void OnClose(IWebSocketConnection socket);
        void OnBinary(byte[] bytes);
    }
}
