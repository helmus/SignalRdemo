// ReSharper disable CheckNamespace

[assembly: WebActivator.PreApplicationStartMethod(
  typeof(DependencyManager), "Init")]

public static class ProcessActivtor
{
    private static SqlDependencyManager DepMan;
    
    public static void Init()
    {
        DepMan = new SqlDependencyManager();
    }
}