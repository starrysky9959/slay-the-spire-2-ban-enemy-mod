using System;
using System.IO;
using System.Reflection;

namespace BanEnemyMod.BanEnemyModCode.Infrastructure;

internal static class HookTrace
{
    private static readonly object Sync = new();

    private static string LogFilePath
    {
        get
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string? directory = Path.GetDirectoryName(assemblyLocation);
            return Path.Combine(directory ?? AppContext.BaseDirectory, "hook_trace.log");
        }
    }

    public static void Write(string message)
    {
        string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";

        lock (Sync)
        {
            File.AppendAllText(LogFilePath, line);
        }
    }
}
