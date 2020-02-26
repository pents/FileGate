using System;
namespace FileGate.Contracts.Enums
{
    public enum MessageType
    {
        CONNECT,
        PING,
        PONG,
        FILE_LIST_REQUEST,
        FILE_LIST_RESPONSE,
        FILE_REQUEST,
        FILE_RESPONSE,
        DISCONNECT
    }
}
