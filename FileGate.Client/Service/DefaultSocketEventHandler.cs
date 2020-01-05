using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileGate.Client.Service.Abstractions;

namespace FileGate.Client.Service
{
    public class DefaultSocketEventHandler : ISocketClientEventHandler
    {
        private readonly SocketConnector _socketConnector;

        public DefaultSocketEventHandler(SocketConnector socketConnector)
        {
            _socketConnector = socketConnector;
        }

        public virtual void RecieveBynary(MemoryStream stream)
        {
            Console.WriteLine($"Recieved {stream.Length} bytes of data");
        }

        public virtual void RecieveText(MemoryStream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                await reader.ReadToEndAsync();
            }
        }

    }

}
