using System;
using System.IO;

namespace FileGate.Client.Service.Abstractions
{
    public interface ISocketClientEventHandler
    {
        void RecieveBynary(MemoryStream stream);
        void RecieveText(MemoryStream stream);

        void BeforeStart();
        void AfterStart();

        void BeforeStop();
        void AfterStop();

        void BeforeSend(string messageContext);
        void AfterSend(string messageContext);
    }
}
