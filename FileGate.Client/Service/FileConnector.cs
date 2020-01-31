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
            List<FileInfo> infos = new List<FileInfo>();
            var dir = new DirectoryInfo(path ?? ConnectedPath);
            infos.AddRange(dir.GetFiles().Select(file => file));

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(path ?? ConnectedPath);
            foreach (string subdirectory in subdirectoryEntries)
            {
                infos.AddRange(GetFilesInfoList(subdirectory));
            }

            return infos;
        }

        public static FileData GetFileData(string fileId)
        {
            var file = GetFilesInfoList().FirstOrDefault(file => GetHashSha256(file.Name + file.Length.ToString()) == fileId);

            return new FileData
            {
                FileInfo = new Contracts.Entities.FileInfo
                {
                    FullName = file.Name,
                    Id = fileId,
                    Size = file.Length
                },
                Data = File.ReadAllBytes(file.FullName)
            };
        }

        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += string.Format("{0:x2}", x);
            }
            return hashString;
        }

    }
}
