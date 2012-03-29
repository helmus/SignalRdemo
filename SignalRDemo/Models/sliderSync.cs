using System.Threading.Tasks;
using SignalR;

public class MySlideSync : PersistentConnection
{
    protected override Task OnReceivedAsync(string connectionId, string data)
    {
        


        // Broadcast data to all clients
        return Connection.Broadcast(new { connectionId = connectionId, value = data });
    }
}