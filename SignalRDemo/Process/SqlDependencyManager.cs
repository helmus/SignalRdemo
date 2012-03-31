// ReSharper disable CheckNamespace

using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Hosting;
using SignalR;
using SignalR.Hosting.AspNet;
using SignalR.Infrastructure;
using CommandType = System.Data.CommandType;

class SqlDependencyManager : IRegisteredObject
{
    private SqlConnection _sqlConn;
    private readonly string _connectString;
    private static int counter;

    public SqlDependencyManager()
    {
        var myTimer = new System.Timers.Timer {Interval = 500};
        myTimer.Elapsed += executeTimer;
        myTimer.Start();
        counter = 0;

        return;
        // This will make the application pool aware of this object
        HostingEnvironment.RegisterObject(this);
        _connectString = "Data Source=WILLEMPC;Initial Catalog=TestDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        _sqlConn = new SqlConnection(_connectString);
        
        SqlDependency.Start(_connectString);
        
        SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Movies", _sqlConn)
        {
            CommandType = CommandType.Text,
            Notification = null
        };

        SqlDependency dependency = new SqlDependency(cmd);
        dependency.OnChange += OnChange;
    }

    static void executeTimer( object sender, ElapsedEventArgs elapsedEventArgs)
    {
        IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
        IConnection connection = connectionManager.GetConnection<MyConnection>();

        connection.Broadcast(counter.ToString());
        counter += 1;
    }

    static void OnChange(object sender, SqlNotificationEventArgs e)
    {
        SqlDependency dependency = sender as SqlDependency;

        // Notices are only a one shot deal
        // so remove the existing one so a new 
        // one can be added
        if (dependency != null) dependency.OnChange -= OnChange;

        IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
        IConnection connection = connectionManager.GetConnection<MyConnection>();

        connection.Broadcast("Update!");
    }


    public void Stop(bool immediate)
    {
        
       // SqlDependency.Stop(_connectString);
    }
}