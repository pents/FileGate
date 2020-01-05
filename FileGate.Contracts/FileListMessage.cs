using System;
using System.Collections.Generic;

namespace FileGate.Contracts
{
    public class FileListMessage : MessageBase
    {
        public FileListMessage()
        {
            Type = Enums.MessageType.FileListResponse;
            Files = new List<FileInfo>();
        }
        public IList<FileInfo> Files { get; set; }
    }

    public class FileInfo
    {
        public string FullName { get; set; }
        public long Size { get; set; }
    }
}
