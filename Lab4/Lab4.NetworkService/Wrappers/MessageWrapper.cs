using System.Net;
using Snakes;

namespace Lab4.NetworkService.Wrappers;

public class MessageWrapper
{
    public GameMessage Message { get; set; }
    public IPEndPoint EndPoint { get; set; }

    public MessageWrapper(GameMessage message, IPEndPoint sender)
    {
        Message = message;
        EndPoint = sender;
    }
}