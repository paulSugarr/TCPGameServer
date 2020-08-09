using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TCPServer
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        public string userName = "<unnamed>";
        private TcpClient client;
        public ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
            Stream = client.GetStream();
        }



        public void Process()
        {
            while (true)
            {

                try
                {
                    var command = GetMessage();
                    CommandManager.TryExecute(command, this);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }
            server.RemoveConnection(this.Id);
            Close();
        }

        private byte[] GetMessage()
        {
            byte[] data = new byte[64];
            int bytes = 0;
            int i = 0;
            
            do
            {
                i++;
                bytes = Stream.Read(data, 0, data.Length);

            }
            while (Stream.DataAvailable && client.Client.Connected);
            return data;
        }

        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
