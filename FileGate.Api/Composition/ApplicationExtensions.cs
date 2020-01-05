using System;
using Microsoft.Extensions.DependencyInjection;
using FileGate.Application.Services.Abstractions;
using FileGate.Application.Services;

namespace FileGate.Api.Composition
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationDependensies(this IServiceCollection services)
        {
            return services
                .AddSingleton<ISocketServerEventHandler, SocketServerEventHandler>()
                .AddSingleton<ISocketServer, SocketServer>();
        }
    }
}
