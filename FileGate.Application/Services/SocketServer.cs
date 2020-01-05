using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileGate.Application.Services.Abstractions;
using FileGate.Application.Configuration;
using Fleck;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FileGate.Application.Services
{
    public class SocketServer : ISocketServer
    {
        private readonly ServerConfiguration _serverOptions;
        private readonly IWebSocketServer _socketServer;
        private readonly ISocketServerEventHandler _eventHandler;

        public SocketServer(IOptions<ServerConfiguration> serverOptions, ISocketServerEventHandler eventHandler)
        {
            _serverOptions = serverOptions.Value;
            var server = new WebSocketServer($"ws://{_serverOptions.IP}:{_serverOptions.Port}/socket");
            _socketServer = server;
            _eventHandler = eventHandler;
        }

        public Task Send<TIn>(TIn data, Guid clientId)
        {
            var socket = _eventHandler.GetSocket(clientId);

            socket.Send(JsonConvert.SerializeObject(data));

            return Task.CompletedTask;
        }

        public async Task<TResult> SendWithResult<TResult, TIn>(TIn data, Guid clientId)
        {
            var socket = _eventHandler.GetSocket(clientId);

            var currentHandler = socket.OnMessage;

            socket.OnMessage -= currentHandler;

            TResult result = default;

            socket.OnMessage += (message) =>
            {
                result = JsonConvert.DeserializeObject<TResult>(message);

                socket.OnMessage -= (Action<string>)socket.OnMessage.GetInvocationList()[0];

                socket.OnMessage += currentHandler;
            };

            await socket.Send(JsonConvert.SerializeObject(data));

            return result;
        }

        public void Start()
        {
            _socketServer.Start(socket =>
            {
                socket.OnError = _eventHandler.OnError;
                socket.OnMessage = (message) => _eventHandler.OnMessage(message, socket);
                socket.OnBinary = _eventHandler.OnBinary;
                socket.OnOpen = () => _eventHandler.OnOpen(socket);
                socket.OnClose = () => _eventHandler.OnClose(socket);
            });
        }

        public void Stop()
        {
            _socketServer.Dispose();
        }

    }
}
