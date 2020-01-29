using System;
namespace FileGate.Application.Services.RequestHandlers
{
    public interface IRequestHandler
    {
        TResult Handle<TResult>(string message);
    }
}
