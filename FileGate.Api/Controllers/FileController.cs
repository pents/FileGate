using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileGate.Application.Services.Abstractions;
using FileGate.Contracts;
using FileGate.Contracts.Dto;
using FileGate.Contracts.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileGate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/{UserId}")]
    public class FileController : ControllerBase
    {
        private readonly ISocketServer _socketServer;

        public FileController(ISocketServer socketServer)
        {
            _socketServer = socketServer;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FileInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFileList(Guid userId)
        {
            var fileList = await _socketServer.SendWithResult<IEnumerable<FileInfo>>(
                userId,
                new MessageBase { Type = Contracts.Enums.MessageType.FILE_LIST_REQUEST });
            
            return Ok(fileList);
        }

        [HttpGet]
        [Route("{fileIdentifier}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFile(Guid userId, string fileIdentifier)
        {
            var fileData = await _socketServer.SendWithResult<FileData>(userId, new FileDataRequestDto
            {
                Hash = fileIdentifier,
                Type = Contracts.Enums.MessageType.FILE_REQUEST
            });

            return File(fileData.Data, "application/octet-stream", fileData.FileInfo.FullName);
        }
    }
}
