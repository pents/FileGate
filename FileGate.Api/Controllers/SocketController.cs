using System.Threading.Tasks;
using FileGate.Application.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FileGate.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SocketController : ControllerBase
    {
        private readonly ISocketServer _socketServer;

        public SocketController(ISocketServer socketServer)
        {
            _socketServer = socketServer;
        }

        public async Task<IActionResult> Connect()
        {
            await _socketServer.ReceiveConnection();

            return StatusCode(StatusCodes.Status101SwitchingProtocols);
        }

    }
}
