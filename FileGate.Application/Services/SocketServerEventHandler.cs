using System;
using System.Collections.Generic;
using System.Text;
using FileGate.Application.Services.Abstractions;
using FileGate.Contracts;
using FileGate.Contracts.Enums;
using Fleck;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FileGate.Application.Services
{
    public class SocketServerEventHandler : ISocketServerEventHandler
    {
        private readonly ILogger<SocketServerEventHandler> _logger;
        private readonly Dictionary<Guid, IWebSocketConnection> _connectedClients;

        public SocketServerEventHandler(ILogger<SocketServerEventHandler> logger)
        {
            _logger = logger;
            _connectedClients = new Dictionary<Guid, IWebSocketConnection>();
        }

        public virtual IWebSocketConnection GetSocket(Guid clientId)
        {
            return _connectedClients[clientId];
        }

        public virtual void OnBinary(byte[] bytes)
        {
            _logger.LogWarning($"Socket server binary input: {Encoding.UTF8.GetString(bytes)}");
        }

        public virtual void OnClose(IWebSocketConnection socket)
        {
            _logger.LogInformation($"Client disconencted {socket.ConnectionInfo.Id}");
            _connectedClients.Remove(socket.ConnectionInfo.Id);
        }

        public virtual void OnError(Exception ex)
        {
            _logger.LogError($"Socket error: {ex.Message}");
        }

        public virtual void OnMessage(string message, IWebSocketConnection socket)
        {
            _logger.LogInformation($"Socket message recieved: {message}");

            var convertedMessage = JsonConvert.DeserializeObject<MessageBase>(message);

            switch (convertedMessage.Type)
            {
                case MessageType.Connect:
                    OnConnect(message, socket);
                    break;
            }
        }

        public virtual void OnOpen(IWebSocketConnection socket)
        {
            _logger.LogInformation($"New client connection {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort} --- {socket.ConnectionInfo.Id}");
        }


        public void OnConnect(string message, IWebSocketConnection socket)
        {
            var connectMessage = JsonConvert.DeserializeObject<ClientInfoMessage>(message);

            _connectedClients.Add(connectMessage.ClientId, socket);
        }
    }
}
