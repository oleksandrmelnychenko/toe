using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tic_tac_toe_Server.Net
{
    public class Client : TcpClient, IDisposable
    {
        public Guid Id { get; set; }

        public Client(TcpClient tcpClient)
        {
            Client = tcpClient.Client;
            Id = Guid.NewGuid();
        }

    }
}
