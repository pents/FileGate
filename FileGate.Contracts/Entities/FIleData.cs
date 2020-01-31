using System;
namespace FileGate.Contracts.Entities
{
    public class FileData
    {
        public FileInfo FileInfo { get; set; }
        public byte[] Data { get; set; }
    }
}
