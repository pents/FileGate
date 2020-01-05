using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileGate.Client.Extensions;
using FileGate.Contracts;
using Newtonsoft.Json;

namespace FileGate.Client.Service
{
    public class SocketConnector
    {
        private Guid _currentSocketId;
        private static Guid _currentClientId = Guid.NewGuid();
        private const int bufferSize = 1024;
        private CancellationTokenSource tokenSource;

        public SocketConnector()
        {
            tokenSource = new CancellationTokenSource();
        }


        public async Task StartClient()
        {
            using (var _client = new ClientWebSocket())
            {
                Console.WriteLine($"Current state {_client.State}");
                await _client.ConnectAsync(new Uri("ws://127.0.0.1:10001/socket"), tokenSource.Token);

                await _client.Send(new ClientInfoMessage
                {
                    ClientId = _currentClientId,
                    
                }, token: tokenSource.Token);

            }

        }

        public void StopClient()


         
    }
}
