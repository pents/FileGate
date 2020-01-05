using System;
namespace FileGate.Contracts.Enums
{
    public enum MessageType
    {
        Connect,
        Ping,
        Pong,
        FileListRequest,
        FileListResponse,
        FileRequest,
        FileResponse
    }
}
