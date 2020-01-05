using System;
using System.Collections.Generic;
using System.IO;

namespace FileGate.Client.Service
{
    public static class FileConnector
    {
        public static string ConnectedPath;


        public static IEnumerable<FileInfo> GetFilesInfoList(string path = null)
        {

            List<FileInfo> infos = new List<FileInfo>();
            var dir = new DirectoryInfo(path ?? ConnectedPath);
            FileInfo[] fileEntries = dir.GetFiles();
            foreach (var fileInfo in fileEntries)
            {
                infos.Add(fileInfo);
            }
                

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(path ?? ConnectedPath);
            foreach (string subdirectory in subdirectoryEntries)
            {
                infos.AddRange(GetFilesInfoList(subdirectory));
            }

            return infos;
        }
    }
}
