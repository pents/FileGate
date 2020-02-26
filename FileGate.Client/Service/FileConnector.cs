using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using FileData = FileGate.Contracts.Entities.FileData;
using System.Text;
using System.Security.Cryptography;

namespace FileGate.Client.Service
{
    public static class FileConnector
    {
        public static string ConnectedPath;


        public static IEnumerable<FileInfo> GetFilesInfoList(string path = null)
        {
            var infos = new List<FileInfo>();
            var dir = new DirectoryInfo(path ?? ConnectedPath);
            infos.AddRange(dir.GetFiles().Select(file => file));

            // Recurse into subdirectories of this directory.
            var subdirectoryEntries = Directory.GetDirectories(path ?? ConnectedPath);
            foreach (var subdirectory in subdirectoryEntries)
            {
                infos.AddRange(GetFilesInfoList(subdirectory));
            }

            return infos;
        }

        public static FileData GetFileData(string fileId)
        {
            var _file = GetFilesInfoList().FirstOrDefault(file => GetHashSha256(file.Name + file.Length) == fileId);

            return new FileData
            {
                FileInfo = new Contracts.Entities.FileInfo
                {
                    FullName = _file.Name,
                    Id = fileId,
                    Size = _file.Length
                },
                Data = File.ReadAllBytes(_file.FullName)
            };
        }

        public static string GetHashSha256(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var hashstring = new SHA256Managed();
            var hash = hashstring.ComputeHash(bytes);
            return hash.Aggregate(string.Empty, (current, x) => current + $"{x:x2}");
        }

    }
}
