using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using FileGate.Client.Configuration;
using FileGate.Client.Service;

namespace FileGate.Client
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            CommandLineArgs commandLineArgs = null;

            Parser.Default.ParseArguments<CommandLineArgs>(args)
                .WithParsed(options =>
                {
                    Console.WriteLine($"Start sharing {options.Path}");
                    commandLineArgs = options;
                })
                .WithNotParsed(options =>
                {
                    Console.WriteLine("Error -- Path is not correct");
                    Console.ReadLine();
                });

            if (commandLineArgs != null)
            {
                FileConnector.ConnectedPath = commandLineArgs.Path;
                var connector = new SocketConnector();
                await connector.StartClient(new Uri("ws://127.0.0.1:10001/socket"));
             
                while (true)
                {
                    var command = Console.ReadLine();
                    if (command == ".exit")
                    {
                        break;
                    }
                }

                await connector.StopClient();
            }
        }

    }
}
