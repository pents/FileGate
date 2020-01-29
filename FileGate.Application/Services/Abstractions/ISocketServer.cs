using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileGate.Application.Services.Abstractions
{
    public interface ISocketServer
    {
        Task ReceiveConnection();
        Task Send(Guid clientId, string message);
        Task<TResult> SendWithResult<TResult>(Guid clientId, string message);
        Task<TResult> SendWithResult<TResult>(Guid clientId, object message);
        Task<TResult> Receive<TResult>(Guid clientId);
        Task<ISocketConnection> GetSocket(Guid clientId);
    }
}
