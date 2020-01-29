using System;
using FileGate.Api.Dto.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileGate.Api.Composition
{
    public static class ApplicationOptions
    {
        public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<ServerConfiguration>(configuration.GetSection("ServerConfiguration"));
        }
    }
}
