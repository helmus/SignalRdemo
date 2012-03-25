using System.Threading.Tasks;
using SignalR;

public class MyConnection : PersistentConnection
{
    protected override Task OnReceivedAsync(string connectionId, string data)
    {
        // Broadcast data to all clients
        return Connection.Broadcast(data);
    }

}