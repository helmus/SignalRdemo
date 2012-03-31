// ReSharper disable CheckNamespace

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Permissions;
using System.Timers;
using System.Web.Hosting;
using SignalR;
using SignalR.Hosting.AspNet;
using SignalR.Infrastructure;

class SqlDependencyManager : IRegisteredObject
{
    private SqlConnection _sqlConn;
    private readonly string _connectString;
    private static int counter;

    public SqlDependencyManager()
    {
        // This will make the application pool aware of this object
        HostingEnvironment.RegisterObject(this);
        _connectString = ConfigurationManager.ConnectionStrings["testDB"].ConnectionString;

        StartNotifyConnection();
    }

    private void StartNotifyConnection()
    {
        _sqlConn = new SqlConnection(_connectString);

        // check if hosting enviromnet can listen to exceptions
        SqlClientPermission perm =new SqlClientPermission(PermissionState.Unrestricted);

        try
        {
            perm.Demand();
        }
        catch (Exception ex)
        {
            var secEx = new Exception("The hosting enviromnent does not have SqlClientPermission",ex);
            broadCastException(secEx);
            throw secEx;
        }

        try
        {
            _sqlConn.Open();
            SqlDependency.Stop(_connectString);
            SqlDependency.Start(_connectString);
            StartNotifying();
        }
        catch (Exception ex)
        {
            broadCastException(ex);
            throw;
        }
    }
    
    /// <summary>
    /// This method will notify connected users of an exception that occured
    /// </summary>
    /// <param name="ex"></param>
    private void broadCastException( Exception ex  )
    {
        IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
        IConnection connection = connectionManager.GetConnection<MyConnection>();

        connection.Broadcast(ex);
    }

    private void StartNotifying()
    {
        string query = "SELECT dbo.movies.Id, dbo.movies.name, dbo.movies.producerID, dbo.movies.releaseDate FROM dbo.movies";
        SqlCommand cmd = new SqlCommand(query, _sqlConn)
        {
            CommandType = System.Data.CommandType.Text,
            Connection = _sqlConn
        };

        SqlDependency dependency = new SqlDependency(cmd);
        dependency.OnChange +=  OnChange;

        cmd.ExecuteNonQuery();
    }

    private void OnChange(object sender, SqlNotificationEventArgs e)
    {
        SqlDependency dependency = (SqlDependency)sender;

        // Notices are only a one shot deal
        // so remove the existing one so a new 
        // one can be added
        if (dependency != null) dependency.OnChange -= OnChange;
        
        IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
        IConnection connection = connectionManager.GetConnection<MyConnection>();

        connection.Broadcast("Update!");

        try
        {
            StartNotifying();
        }
        catch (Exception ex)
        {
            broadCastException(ex);
            throw;
        }

    }

    static void executeTimer(object sender, ElapsedEventArgs elapsedEventArgs)
    {
        IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
        IConnection connection = connectionManager.GetConnection<MyConnection>();

        connection.Broadcast(counter.ToString());
        counter += 1;
    }

    public void Stop(bool immediate)
    {
        SqlDependency.Stop(_connectString);
        if (_sqlConn != null) _sqlConn.Close();
    }
}