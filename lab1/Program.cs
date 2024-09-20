using System.Net;
using System.Net.Sockets;
using System.Text;

string multicastAddress = "224.0.0.1";
int port = 12345;
Guid self = Guid.NewGuid();

if (args.Length > 1) multicastAddress = args[1];

using (UdpClient udpClient = new UdpClient())
{ 
    udpClient.ExclusiveAddressUse = false; 
    udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); 
    udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port)); 
    udpClient.JoinMulticastGroup(IPAddress.Parse(multicastAddress));
    Dictionary<Guid, TimeOnly> aliveCopies = new Dictionary<Guid, TimeOnly>(); 
    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(multicastAddress), port);

    void RegisterCopy(Guid guid)
    {
        object loc = new object();
        lock (loc)
        {
            if (!aliveCopies.TryAdd(guid, TimeOnly.FromDateTime(DateTime.Now)))
            {
                aliveCopies[guid] = TimeOnly.FromDateTime(DateTime.Now);
            }
        }
    }

    void PrintAliveCopies()
    {
        object loc = new object();
        lock (loc)
        {
            TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);
            Dictionary<Guid, TimeOnly> checkedAliveCopies = new Dictionary<Guid, TimeOnly>();
            foreach (var pair in aliveCopies)
            {
                if ((now - pair.Value) < TimeSpan.FromSeconds(3))
                {
                    checkedAliveCopies.Add(pair.Key, pair.Value);
                }
            }
            aliveCopies = checkedAliveCopies;
            foreach (var pair in aliveCopies)
            {
                Console.WriteLine("{0} still alive", pair.Key);
            }
        }
    }
    
    async void GetCopies() 
    { 
        while (true) 
        { 
            var receivedBytes = await udpClient.ReceiveAsync(); 
            var strRepresent = Encoding.UTF8.GetString(receivedBytes.Buffer);
            Guid receivedGuid = Guid.Parse(strRepresent.Substring(8));
            if(receivedGuid == self) continue;
            RegisterCopy(receivedGuid);
        } 
    }

    void Send() 
    { 
        byte[] message = Encoding.UTF8.GetBytes("IM ALIVE" + self); 
        udpClient.Send(message, message.Length, remoteEndPoint);
    }

    Timer sendTimer = new Timer((object? obj) => {Send();}, null, 0, 1000);
    Timer printTimer = new Timer((object? obj) => { PrintAliveCopies(); }, null, 0, 4000);
    GetCopies();
    
    while (true) 
    { 
        Thread.Sleep(1000); 
    }
}
