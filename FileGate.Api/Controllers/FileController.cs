using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileGate.Application.Services.Abstractions;
using FileGate.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FileGate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/{UserId}")]
    public class FileController : Controller
    {
        private readonly ISocketServer _socketServer;

        public FileController(ISocketServer socketServer)
        {
            _socketServer = socketServer;
        }

        [HttpGet]
        [ProducesResponseType(typeof(FileListMessage), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFileList(Guid userId)
        {
            var fileList = await _socketServer.SendWithResult<FileListMessage, MessageBase>(new MessageBase
            {
                Type = Contracts.Enums.MessageType.FileListRequest
            }, userId);
            return Ok(fileList);
        }

        //[HttpGet]
        //[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        //public IActionResult GetFile(Guid UserId, [FromQuery]FileIdentifier fileIdentifier)
        //{

        //}
    }
}
