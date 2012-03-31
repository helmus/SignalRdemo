// ReSharper disable CheckNamespace

using System;

[assembly: WebActivator.PostApplicationStartMethod(
  typeof(ProcessActivtor), "Init")]

public static class ProcessActivtor
{
    private static SqlDependencyManager DepMan;
    
    public static void Init()
    {
        bool connected = false;
        while (!connected)
        {
            try
            {
                DepMan = new SqlDependencyManager();
                connected = true;
            }
            catch (Exception)
            {
                // this way we don't have to wait until the next recycle or restart
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}