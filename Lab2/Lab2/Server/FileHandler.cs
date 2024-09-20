using System.Net.Sockets;
using System.Text;

namespace Lab2.Server;

public class FileHandler : IDisposable
{
    private readonly TcpClient _client;
    private FileStream _fileStream;
    private string _fileName;
    private long _fileSize;
    private long _readSize;
    private long _lastReadSize;
    private TimeOnly startTime;
    
    public FileHandler(TcpClient client)
    {
        _client = client;
    }

    public async void HandleConnection()
    {
        using (this)
        {
            int chunkSize = 8192;
            var stream = _client.GetStream();
            byte[] buffer = new byte[chunkSize];
            await stream.ReadAsync(buffer, 0, chunkSize);
            ParseFileMetadata(buffer);
            CreateFile();
            await stream.WriteAsync(Encoding.ASCII.GetBytes("ok"), 0, "ok".Length);
            startTime = TimeOnly.FromDateTime(DateTime.Now);
            Timer timer = new Timer((object? o) => { PrintSpeed(); }, null, 3000, 3000);
            while (_readSize < _fileSize)
            {
                int count = stream.ReadAsync(buffer, 0, chunkSize).Result;
                await _fileStream.WriteAsync(buffer, 0, count);
                _readSize += count;
            }

            timer.Change(Timeout.Infinite, Timeout.Infinite);
            await stream.WriteAsync(Encoding.ASCII.GetBytes("done"), 0, "done".Length);
            PrintSpeed();
        }
    }

    private void ParseFileMetadata(byte[] buffer)
    {
        string stringifiedBuffer = Encoding.Default.GetString(buffer);
        
        //slash is deprecated to use in both windows and unix file names, so I can use it as a split character
        var data = stringifiedBuffer.Split('/');
        _fileName = data[0];
        _fileSize = long.Parse(data[1]);
    }
    private void CreateFile()
    {
        string fullFileName;
        if (!File.Exists("uploads/" + _fileName))
        {
            fullFileName = "uploads/" + _fileName;
        }
        else
        {
            fullFileName = "uploads/" + _fileName + Random.Shared.Next();
        }
        _fileStream = new FileStream(fullFileName, FileMode.CreateNew);
    }

    private void PrintSpeed()
    {
        TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);
        double totalSpeed = _readSize / (now - startTime).TotalSeconds / 1024 / 1024;
        double currentSpeed = (double)(_readSize - _lastReadSize) / double.Min((now - startTime).TotalSeconds, 3) / 1024 / 1024;
        _lastReadSize = _readSize;
        Console.WriteLine("общая скорость {0:F2} мнгновенная скорость {1:F2} Мб", totalSpeed, currentSpeed);
    }

    public void Dispose()
    {
        _fileStream.Close();
        _client.Close();
    }
}