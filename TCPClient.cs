using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1
{
    class TCPClient
    {
        public static NetworkStream networkStream { get; set; }
        public TCPClient()
        {

            TcpClient tcpClient = new TcpClient("116.62.171.238", 8888);
            networkStream = tcpClient.GetStream();
            Thread thread = new Thread(ReceiveData);
            thread.Start(networkStream);
           
        }
        public static void ReceiveData()
        {
            byte[] data = new byte[256];
            String responseData = String.Empty;
            while (true)
            {
                Int32 bytes = networkStream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

            }
        }
    }
}
