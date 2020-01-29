using System;
using System.Net.WebSockets;
using FileGate.Application.Services.Abstractions;
using FileGate.Contracts;
using Newtonsoft.Json;

namespace FileGate.Application.Services.RequestHandlers
{
    public class ConnectionRequestHandler : IRequestHandler
    {
        private readonly ISocketConnection _socket;

        public ConnectionRequestHandler(ISocketConnection socket)
        {
            _socket = socket;
        }

        public TResult Handle<TResult>(string message)
        {
            return JsonConvert.DeserializeObject<TResult>(message);
        }
    }
}
