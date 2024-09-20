using System.Net;
using System.Net.Sockets;

namespace Lab2.Server;

public class Server
{
    private int _port;

    public Server(int port)
    {
        _port = port;
        SetupDirectoryForFiles();
    }

    private void SetupDirectoryForFiles()
    {
        if (!Directory.Exists("uploads"))
        {
            Directory.CreateDirectory("uploads");
        }
    }
    

    public void StartServer()
    {
        using TcpListener listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        while (true)
        { 
            TcpClient newClient = listener.AcceptTcpClientAsync().Result;
            FileHandler handler = new FileHandler(newClient);
            try
            {
                Task.Run(handler.HandleConnection);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad happened");
            }
        }
    }
}