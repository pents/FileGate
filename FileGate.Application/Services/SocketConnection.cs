using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileGate.Application.Services.Abstractions;
using Newtonsoft.Json;

namespace FileGate.Application.Services
{
    public class SocketConnection : ISocketConnection
    {
        private readonly WebSocket _currentSocket;
        private readonly int _bufferSize;
        private readonly CancellationToken _token;
        private string _currentMessage;

        public SocketConnection(WebSocket socket) : this(socket, 1024, CancellationToken.None)
        {
        }

        public SocketConnection(WebSocket socket, int bufferSize) : this(socket, bufferSize, CancellationToken.None)
        {
        }

        public SocketConnection(WebSocket socket, int bufferSize, CancellationToken token)
        {
            _currentSocket = socket;
            _bufferSize = bufferSize;
            _token = token;
        }

        public async Task<TResult> GetMessage<TResult>()
        {
            if (string.IsNullOrEmpty(_currentMessage))
            {
                _currentMessage = await Receive();
            }
            var result = JsonConvert.DeserializeObject<TResult>(_currentMessage);
            _currentMessage = null;
            return result;
        }

        public async Task Send(string message)
        {
            var byteData = Encoding.UTF8.GetBytes(message);
            var bytesCount = byteData.Length;
            var currentByte = 0;

            while (currentByte <= bytesCount)
            {
                bool lastChunk = currentByte + _bufferSize > bytesCount;
                await _currentSocket.SendAsync(
                    new ArraySegment<byte>(byteData, currentByte, lastChunk ? bytesCount : _bufferSize),
                    WebSocketMessageType.Text,
                    lastChunk,
                    _token);


                currentByte += _bufferSize;
            }
        }

        public async Task<TResult> SendWithResult<TResult>(string message)
        {
            await Send(message);

            return await OnMessage<TResult>();
        }

        public async Task Listen()
        {
            while(true && !_token.IsCancellationRequested)
            {
                _currentMessage = await Receive();
            }
        }

        private async Task<TResult> OnMessage<TResult>()
        {
            while (string.IsNullOrEmpty(_currentMessage))
            {
                await Task.Delay(5);
            }

            return await GetMessage<TResult>();
        }

        private async Task<string> Receive()
        {
            var buffer = new ArraySegment<byte>(new byte[_bufferSize]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await _currentSocket.ReceiveAsync(buffer, _token);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }

            }
        }
    }
}
