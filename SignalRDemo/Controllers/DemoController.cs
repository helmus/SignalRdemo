using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SignalR.Infrastructure;
using SignalR.Hosting.AspNet;
using SignalR;


// ReSharper disable CheckNamespace
public class DemoController : Controller
{
    public void SendMessage( string message)
    {
        IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
        IConnection connection = connectionManager.GetConnection<MyConnection>();

        connection.Broadcast(message);
    }
}
