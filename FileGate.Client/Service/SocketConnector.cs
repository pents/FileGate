using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileGate.Client.Service.Abstractions;
using FileGate.Contracts;
using FileGate.Contracts.Dto;
using Newtonsoft.Json;

namespace FileGate.Client.Service
{
    public class SocketConnector
    {
        private string _currentClientId;
        private readonly CancellationTokenSource _tokenSource;
        private readonly ClientWebSocket _client;
        private readonly int _bufferSize;
        private ISocketClientEventHandler _eventHandler;

        #region events
        private event Action OnBeforeStart;
        private event Action OnAfterStart;
        private event Action OnBeforeStop;
        private event Action OnAfterStop;
        private event Action<string> OnBeforeSend;
        private event Action<string> OnAfterSend;
        private event Action<MemoryStream> OnRecieveText;
        private event Action<MemoryStream> OnRecieveBynary;
        #endregion

        public SocketConnector(int bufferSize = 1024, ISocketClientEventHandler clientEventHandler = null)
        {
            _bufferSize = bufferSize;
            _tokenSource = new CancellationTokenSource();
            _client = new ClientWebSocket();

            _eventHandler = clientEventHandler ?? new DefaultSocketEventHandler(this);

            OnBeforeStart += _eventHandler.BeforeStart;
            OnAfterStart += _eventHandler.AfterStart;

            OnBeforeStop += _eventHandler.BeforeStop;
            OnAfterStop += _eventHandler.AfterStop;

            OnBeforeSend += _eventHandler.BeforeSend;
            OnAfterSend += _eventHandler.AfterSend;

            OnRecieveText += _eventHandler.RecieveText;
            OnRecieveBynary += _eventHandler.RecieveBynary;
        }

        public async Task StartClient(Uri uri)
        {
            OnBeforeStart?.Invoke();
            await _client.ConnectAsync(uri, _tokenSource.Token);
            //Console.WriteLine($"Current file URL: http://localhost:8095/File/{_currentClientId}");
   
            StartListening(_client, token: _tokenSource.Token).ConfigureAwait(false);

            OnAfterStart?.Invoke();
        }

        public async Task Send(string data)
        {
            OnBeforeSend?.Invoke(data);

            var byteData = Encoding.UTF8.GetBytes(data);
            var bytesCount = byteData.Length;
            var currentByte = 0;

            while (currentByte <= bytesCount)
            {
                bool lastChunk = currentByte + _bufferSize > bytesCount;

                var count = lastChunk ? bytesCount - currentByte : _bufferSize;

                await _client.SendAsync(
                    new ArraySegment<byte>(byteData, currentByte, count),
                    WebSocketMessageType.Text,
                    lastChunk,
                    _tokenSource.Token);


                currentByte += _bufferSize;
            }

            OnAfterSend?.Invoke(data);
        }

        public async Task Send<T>(T sendObj)
        {
            var textData = JsonConvert.SerializeObject(sendObj);

            await Send(textData);
        }

        public Task StopClient()
        {
            OnBeforeStop?.Invoke();
            _tokenSource.Cancel();
            _client.Dispose();
            OnAfterStop?.Invoke();
            return Task.CompletedTask;
        }

        public void SetClientId(string clientId)
        {
            _currentClientId = clientId;
        }

        private async Task StartListening(WebSocket socket, int bufferSize = 1024, CancellationToken token = default)
        {
            while(!token.IsCancellationRequested)
            {
                await Recieve(socket, bufferSize, token);
            }
        }

        private async Task Recieve(WebSocket socket, int bufferSize = 1024, CancellationToken token = default)
        {
            var buffer = new ArraySegment<byte>(new byte[bufferSize]);
            using(var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await socket.ReceiveAsync(buffer, token);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        OnRecieveText?.Invoke(ms);
                        break;
                    case WebSocketMessageType.Binary:
                        OnRecieveBynary?.Invoke(ms);
                        break;
                }
            }
        }

    }
}
