using System.Net;
using System.Net.Sockets;

namespace Lab4.NetworkService.Wrappers;

public class UdpClientWrapper : IDisposable
{
    private readonly UdpClient _client;

    public UdpClientWrapper(UdpClient client)
    {
        _client = client;
    }
    
    public async Task SendAsync(byte[] data, IPEndPoint endPoint) => await _client.SendAsync(data, data.Length, endPoint);
    
    public async Task SendAsync(byte[] data) => await _client.SendAsync(data, data.Length);
    
    public async Task<UdpReceiveResult> ReceiveAsync() => await _client.ReceiveAsync();

    public void Dispose()
    {
        _client.Dispose();
    }
}