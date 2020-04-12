using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileGate.Application.Services.Abstractions
{
    public interface ISocketServer
    {
        Task ReceiveConnection();
        Task Send(string clientId, string message);
        Task<TResult> SendWithResult<TResult>(string clientId, string message);
        Task<TResult> SendWithResult<TResult>(string clientId, object message);
        Task<TResult> Receive<TResult>(string clientId);
        Task<ISocketConnection> GetSocket(string clientId);
    }
}
