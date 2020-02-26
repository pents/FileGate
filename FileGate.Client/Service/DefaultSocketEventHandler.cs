using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileGate.Client.Service.Abstractions;
using FileGate.Contracts;
using FileGate.Contracts.Dto;
using FileGate.Contracts.Enums;
using Newtonsoft.Json;

namespace FileGate.Client.Service
{
    public class DefaultSocketEventHandler : ISocketClientEventHandler
    {
        private readonly SocketConnector _socketConnector;

        public DefaultSocketEventHandler(SocketConnector socketConnector)
        {
            _socketConnector = socketConnector;
        }

        public void AfterSend(string messageContext)
        {
            Console.WriteLine($"Message sent");
        }

        public void AfterStart()
        {
            Console.WriteLine("Client started");
        }

        public void AfterStop()
        {
            Console.WriteLine("Client stopped");
        }

        public void BeforeSend(string messageContext)
        {
            Console.WriteLine($"Message is sending: {messageContext}");
        }

        public void BeforeStart()
        {
            Console.WriteLine("Client is starting");
        }

        public void BeforeStop()
        {
            Console.WriteLine("Client is stopping");
        }

        public virtual void RecieveBynary(MemoryStream stream)
        {
            Console.WriteLine($"Recieved {stream.Length} bytes of data");
        }

        public virtual void RecieveText(MemoryStream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var messageText = reader.ReadToEnd();
                var recievedMessage = JsonConvert.DeserializeObject<MessageBase>(messageText);
                // new RequestHandlerBuilder(recievedMessage.Type).Handle().ConfigureAwait(false);
                // instead of this:
                switch (recievedMessage.Type)
                {
                    case MessageType.CONNECT:
                        OnConnectMessage().ConfigureAwait(false);
                        break;

                    case MessageType.PING:
                        OnPing().ConfigureAwait(false);
                        break;

                    case MessageType.FILE_LIST_REQUEST:
                        OnFileListRequest().ConfigureAwait(false);
                        break;

                    case MessageType.FILE_REQUEST:
                        var fileRequest = JsonConvert.DeserializeObject<FileDataRequestDto>(messageText);
                        OnFileRequest(fileRequest.Hash).ConfigureAwait(false);
                        break;
                }
            }
        }

        private async Task OnFileRequest(string fileId)
        {
            var filedata = FileConnector.GetFileData(fileId);

            await _socketConnector.Send(JsonConvert.SerializeObject(filedata));
        }

        private async Task OnFileListRequest()
        {
            var files = FileConnector.GetFilesInfoList();

            var fileList = files.Select((file) => new Contracts.Entities.FileInfo
            {
                Id = FileConnector.GetHashSha256(file.Name + file.Length),
                FullName = file.Name,
                Size = file.Length
            });

            await _socketConnector.Send(JsonConvert.SerializeObject(fileList));
        }

        private async Task OnPing()
        {
            await _socketConnector.Send(new MessageBase { Type = MessageType.PONG });
        }

        private async Task OnConnectMessage()
        {
            await _socketConnector.Send(new ClientInfoDto
            {
                ClientId = _socketConnector.GetCurrentClientId(),
                Type = MessageType.CONNECT
            });
        }


    }

}
