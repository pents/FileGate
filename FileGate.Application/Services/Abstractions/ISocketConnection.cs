using System;
using System.Threading.Tasks;

namespace FileGate.Application.Services.Abstractions
{
    public interface ISocketConnection
    {
        Task Send<T>(T message);
        Task Send(string message);
        Task<TResult> SendWithResult<TResult>(string message);
        Task<TResult> GetMessage<TResult>();
        Task Listen();
    }
}
