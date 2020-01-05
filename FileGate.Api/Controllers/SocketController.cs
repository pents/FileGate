using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileGate.Contracts;
using FileGate.Contracts.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileGate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SocketController : Controller
    {
        private readonly Dictionary<Guid, WebSocket> _connectedClients;

        public SocketController()
        {
            _connectedClients = new Dictionary<Guid, WebSocket>();
        }

        [HttpGet]
        public async Task<IActionResult> Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Recieve(socket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task Recieve(WebSocket socket, int bufferSize = 1024, CancellationToken token = default)
        {
            var buffer = new ArraySegment<byte>(new byte[bufferSize]);
            using (var ms = new MemoryStream())
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
                        RecieveText(socket, ms);
                        break;
                    case WebSocketMessageType.Binary:
                        RecieveBynary(socket, ms);
                        break;
                }
            }
        }

        private void RecieveText(WebSocket socket, MemoryStream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var messageText = reader.ReadToEnd();
                var recievedMessage = JsonConvert.DeserializeObject<MessageBase>(messageText);
                // new RequestHandlerBuilder(recievedMessage.Type).Handle().ConfigureAwait(false);
                // instead of this:
                switch (recievedMessage.Type)
                {
                    case MessageType.Connect:
                        OnConnectMessage(socket).ConfigureAwait(false);
                        break;

                    case MessageType.Ping:
                        OnPing(socket).ConfigureAwait(false);
                        break;
                }
            }
        }

        private async Task OnPing(WebSocket socket)
        {
            await _socketConnector.Send(new MessageBase { Type = MessageType.Pong });
        }

        private async Task OnConnectMessage(WebSocket socket)
        {
            await _socketConnector.Send(new ClientInfoMessage
            {
                ClientId = _socketConnector.GetCurrentClientId(),
                Type = MessageType.Connect
            });
        }

        private void RecieveBynary(WebSocket socket, MemoryStream stream)
        {
            Console.WriteLine($"Recieved {stream.Length} bytes of data");
        }

    }
}
