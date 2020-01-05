using System;
using CommandLine;

namespace FileGate.Client.Configuration
{
    public class CommandLineArgs
    {
        [Option('p', "path", HelpText = "path to sharing folder", Required = true)]
        public string Path { get; set; }
    }
}
