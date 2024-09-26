using System.Net;
using System.Net.Sockets;
using System.Text;
using Lab5.SocketWrapper;

namespace Lab5.SocketHandlers;

public class ConnectionHandler : ISocketHandler
{
    public void Handle(SocketConnection connection, List<SocketConnection> connections)
    {
        try
        {
            ISocket socket = connection.Client;
            byte[] data = new byte[1024];
            data = socket.Receive(data);
            byte[] address = ((IPEndPoint)socket.GetSocket().LocalEndPoint).Address.GetAddressBytes();
            int clientPort = ((IPEndPoint)socket.GetSocket().LocalEndPoint).Port;
            byte[] clientPortBytes = BitConverter.GetBytes(clientPort);
            address = Utility.Concatinate(address, [clientPortBytes[0], clientPortBytes[1]]);
            if (data[0] != 0x05 || data[2] != 0x00 || data[1] != 0x01 || data[3] == 0x04)
            {
                socket.Send(Utility.Concatinate([0x05, 0x02, 0x00, 0x01], address));
                socket.Close();
            }

            if (data[3] == 0x01)
            {
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress endPointAddress = new IPAddress(data.Skip(4).Take(4).ToArray());
                byte[] endPointBytes = data.Skip(8).Take(2).Reverse().ToArray();
                int port = BitConverter.ToInt16(endPointBytes, 0);
                serverSocket.Connect(new IPEndPoint(endPointAddress, port));
                serverSocket.Blocking = false;
                connection.Server = new DefaultSocketWrapper(serverSocket);
                connection.Handler = new DataHandler();
                socket.Send(Utility.Concatinate([0x05, 0x00, 0x00, 0x01], address));
            }
            else if (data[3] == 0x03)
            {
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                int count = data[4];
                string domain = Encoding.Default.GetString(data.Skip(5).Take(count).ToArray());
                int port = BitConverter.ToInt16(data.Skip(8).Take(2).ToArray(), 0);
                serverSocket.Connect(new DnsEndPoint(domain, port));
                serverSocket.Blocking = false;
                connection.Server = new DefaultSocketWrapper(serverSocket);
                connection.Handler = new DataHandler();
                socket.Send(Utility.Concatinate([0x05, 0x00, 0x00, 0x01], address));
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e.Message);
            ISocket socket = connection.Client;
            byte[] address = ((IPEndPoint)socket.GetSocket().LocalEndPoint).Address.GetAddressBytes();
            if (e.ErrorCode == 11)
            {
                socket.Send(Utility.Concatinate([0x05, 0x04, 0x00, 0x01], address)); //host unreachable
            }
            else
            {
                socket.Send(Utility.Concatinate([0x05, 0x01, 0x00, 0x01], address));
            }
            socket.Close();
        }
    }
}