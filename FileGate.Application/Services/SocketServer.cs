using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using FileGate.Application.Exceptions;
using FileGate.Application.Services.Abstractions;
using FileGate.Contracts.Dto;
using FileGate.Contracts.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FileGate.Application.Services
{
    public class SocketServer : ISocketServer
    {
        private readonly Dictionary<string, ISocketConnection> _connectedClients;
        private readonly IHttpContextAccessor _contextAccessor;

        public SocketServer(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _connectedClients = new Dictionary<string, ISocketConnection>();
        }

        public Task<ISocketConnection> GetSocket(string clientId)
        {
            return Task.FromResult(_connectedClients[clientId]);
        }

        public async Task<TResult> Receive<TResult>(string clientId)
        {
            return await _connectedClients[clientId].GetMessage<TResult>();
        }

        public async Task ReceiveConnection()
        {
            if (_contextAccessor.HttpContext.WebSockets.IsWebSocketRequest)
            {
                var socket = await _contextAccessor.HttpContext.WebSockets.AcceptWebSocketAsync();
                ISocketConnection connection = new SocketConnection(socket);
                var clientId = GenerateClientId();
                _connectedClients.Add(clientId, connection);
                await connection.Send(new ClientInfoDto
                {
                    Type = MessageType.CONNECT,
                    ClientId = clientId
                });
                await connection.Listen();
                _connectedClients.Remove(clientId);
            }
            else
            {
                throw new ReceiveConnectionException("HttpContext request is not containing socket connection");
            }
        }

        public async Task Send(string clientId, string message)
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

        public async Task<TResult> SendWithResult<TResult>(string clientId, string message)
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

        public async Task<TResult> SendWithResult<TResult>(string clientId, object message)
        {
            var serializedObject = JsonConvert.SerializeObject(message);
            return await SendWithResult<TResult>(clientId, serializedObject);
        }

        private string GenerateClientId()
        {
            var buffer = new byte[4];
            var rand = new Random();
            while (true)
            {
                rand.NextBytes(buffer);
                var id = BytesToString(buffer);
                if (_connectedClients.ContainsKey(id))
                {
                    continue;
                }

                return id;
            }
        }

        private string BytesToString(byte[] array)
        {
            var builder = new StringBuilder();  
            for (int i = 0; i < array.Length; i++)  
            {  
                builder.Append(array[i].ToString("x2"));  
            }

            return builder.ToString();
        }
    }
}
