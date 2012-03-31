// ReSharper disable CheckNamespace

[assembly: WebActivator.PostApplicationStartMethod(
  typeof(ProcessActivtor), "Init")]

public static class ProcessActivtor
{
    private static SqlDependencyManager DepMan;
    
    public static void Init()
    {
        DepMan = new SqlDependencyManager();
    }
}