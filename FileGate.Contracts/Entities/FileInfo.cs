using System;
namespace FileGate.Contracts.Entities
{
    public class FileInfo
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public long Size { get; set; }
    }
}
