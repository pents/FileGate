using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using FileGate.Application.Exceptions;
using FileGate.Application.Services.Abstractions;
using FileGate.Contracts.Dto;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FileGate.Application.Services
{
    public class SocketServer : ISocketServer
    {
        private readonly Dictionary<Guid, ISocketConnection> _connectedClients;
        private readonly IHttpContextAccessor _contextAccessor;

        public SocketServer(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _connectedClients = new Dictionary<Guid, ISocketConnection>();
        }

        public Task<ISocketConnection> GetSocket(Guid clientId)
        {
            return Task.FromResult(_connectedClients[clientId]);
        }

        public async Task<TResult> Receive<TResult>(Guid clientId)
        {
            return await _connectedClients[clientId].GetMessage<TResult>();
        }

        public async Task ReceiveConnection()
        {
            if (_contextAccessor.HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket socket = await _contextAccessor.HttpContext.WebSockets.AcceptWebSocketAsync();
                ISocketConnection connection = new SocketConnection(socket);
                var clientInfo = await connection.GetMessage<ClientInfoDto>();
                var clientId = clientInfo.ClientId;
                _connectedClients.Add(clientId, connection);
                await connection.Listen();
                _connectedClients.Remove(clientId);
            }
            else
            {
                throw new ReceiveConnectionException("HttpContext request is not containing socket connection");
            }
        }

        public async Task Send(Guid clientId, string message)
        {
            try
            {
                var client = _connectedClients[clientId];

                await client.Send(message);
            }
            catch (KeyNotFoundException)
            {
                throw new SendMessageException($"Client '{clientId}' is not found");
            }
            catch (WebSocketException)
            {
                throw new SendMessageException($"Connection to client '{clientId}' is closed or aborted");
            }
        }

        public async Task<TResult> SendWithResult<TResult>(Guid clientId, string message)
        {
            try
            {
                var client = _connectedClients[clientId];

                return await client.SendWithResult<TResult>(message);
            }
            catch(KeyNotFoundException)
            {
                throw new SendMessageException($"Client '{clientId}' is not found");
            }
            catch(WebSocketException)
            {
                throw new SendMessageException($"Connection to client '{clientId}' is closed or aborted");
            }

        }

        public async Task<TResult> SendWithResult<TResult>(Guid clientId, object message)
        {
            var serializedObject = JsonConvert.SerializeObject(message);
            return await SendWithResult<TResult>(clientId, serializedObject);
        }
    }
}
