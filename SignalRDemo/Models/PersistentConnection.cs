﻿using System.Threading.Tasks;
using SignalR;

// ReSharper disable CheckNamespace
public class MyConnection : PersistentConnection
// ReSharper restore CheckNamespace
{
    protected override Task OnReceivedAsync(string connectionId, string data)
    {
        // Broadcast data to all clients
        return Connection.Broadcast(data);
    }

}
