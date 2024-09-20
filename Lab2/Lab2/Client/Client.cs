using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab2.Client;

public class Client : IDisposable
{
    private FileStream _fileToSendStream;
    private TcpClient _serverConnection;
    private long _fileSize;
    private string _fileName = null!;

    public Client(string path, IPAddress server, int port)
    {
        _fileSize = new FileInfo(path).Length;
        string[] pathParts = path.Split(new char[] {'\\', '/'});
        _fileName = pathParts.Length > 1 ? pathParts[-1] : pathParts[0];
        _fileToSendStream = new FileStream(path, FileMode.Open);
        _serverConnection = new TcpClient();
        _serverConnection.Connect(server, port);
    }

    public Client(string path, DnsEndPoint server)
    {
        _fileSize = new FileInfo(path).Length;
        string[] pathParts = path.Split(new char[] {'\\', '/'});
        _fileName = pathParts.Length > 1 ? pathParts[-1] : pathParts[0];
        _fileToSendStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        _serverConnection = new TcpClient();
        _serverConnection.Connect(server.Host, server.Port);
    }

    public async Task HandleFile()
    {
        int chunkSize = 8192;
        using var tcpStream = _serverConnection.GetStream();
        long totalRead = 0;
        
        await tcpStream.WriteAsync(Encoding.Default.GetBytes(_fileName + "/" + _fileSize));
        byte[] buffer = new byte[chunkSize];
        int readBytes = await tcpStream.ReadAsync(buffer, 0, chunkSize);
        if (Encoding.Default.GetString(buffer,0 , readBytes) == "ok")
        {
            while (await _fileToSendStream.ReadAsync(buffer, 0, chunkSize) > 0)
            {
                await tcpStream.WriteAsync(buffer);
            }
            Array.Clear(buffer);
            readBytes = await tcpStream.ReadAsync(buffer, 0, chunkSize);
            if (Encoding.Default.GetString(buffer, 0, readBytes) == "done")
            {
                Console.WriteLine("Файл передан удачно");
            }
            else
            {
                Console.WriteLine("Файл не был передан удачно");
                throw new Exception("no done after work");
            }
        }
        else
        {
            Console.WriteLine(readBytes);
            throw new Exception("no ok before work");
        }
    }

    public void Dispose()
    {
        _fileToSendStream.Close();
        _serverConnection.Close();
    }
}