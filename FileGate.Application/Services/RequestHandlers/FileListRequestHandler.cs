using System;
using FileGate.Application.Services.Abstractions;
using Newtonsoft.Json;

namespace FileGate.Application.Services.RequestHandlers
{
    public class FileListRequestHandler : IRequestHandler
    {
        private readonly ISocketConnection _socket;

        public FileListRequestHandler(ISocketConnection socket)
        {
            _socket = socket;
        }

        public TResult Handle<TResult>(string message)
        {
            return JsonConvert.DeserializeObject<TResult>(message);
        }
    }
}
