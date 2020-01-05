using System;
using System.Threading.Tasks;

namespace FileGate.Application.Services.Abstractions
{
    public interface ISocketServer
    {
        void Start();
        void Stop();
        Task<TResult> SendWithResult<TResult, TIn>(TIn data, Guid clientId);
        Task Send<TIn>(TIn data, Guid clientId);
    }
}
