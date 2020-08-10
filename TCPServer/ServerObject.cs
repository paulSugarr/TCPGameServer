using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPServer
{
    public class ServerObject
    {
        private static TcpListener _tcpListener;
        private List<ClientObject> _clients = new List<ClientObject>();

        protected internal void AddConnection(ClientObject clientObject)
        {
            _clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            ClientObject client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                _clients.Remove(client);
        }

        protected internal void Listen()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, 8888);
                _tcpListener.Start();
                Console.WriteLine("Server run");

                while (true)
                {
                    TcpClient tcpClient = _tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this, _clients.Count.ToString());
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void BroadcastMessage(byte[] message, string id)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Id != id) 
                {
                    _clients[i].Stream.Write(message, 0, message.Length);
                }
            }
        }

        protected internal void SendMessage(byte[] message, string id)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Id == id)
                {
                    _clients[i].Stream.Write(message, 0, message.Length);
                }
            }
        }

        protected internal void Disconnect()
        {
            _tcpListener.Stop();

            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
}
