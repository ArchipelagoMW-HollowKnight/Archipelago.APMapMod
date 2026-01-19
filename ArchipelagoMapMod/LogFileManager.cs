using Modding;
using System.Collections.Concurrent;
using UnityEngine;

namespace ArchipelagoMapMod;

#nullable enable

internal static class LogFileManager
{
    public static string ApmmDirectory = Path.Combine(Application.persistentDataPath, "ArchipelagoMapMod");

    private static BlockingCollection<Action>? logRequests;
    private static Thread? logConsumer;

    public static void EnsureStarted()
    {
        if (logRequests != null || logConsumer != null)
        {
            return;
        }
        Directory.CreateDirectory(ApmmDirectory);
        logRequests = new BlockingCollection<Action>();
        logConsumer = new Thread(() =>
        {
            foreach (Action a in logRequests.GetConsumingEnumerable())
            {
                try
                {
                    a();
                }
                catch (Exception e)
                {
                    ArchipelagoMapMod.Instance.LogError($"Error in log request: {e}");
                }
            }
        })
        { IsBackground = true, Priority = System.Threading.ThreadPriority.Lowest };
        logConsumer.Start();

        ModHooks.ApplicationQuitHook += EnsureStopped;
    }

    public static void EnsureStopped()
    {
        if (logRequests == null || logConsumer == null || !logConsumer.IsAlive)
        {
            return;
        }

        try
        {
            logRequests.CompleteAdding();
            logConsumer.Join();
            logRequests.Dispose();
        }
        catch (Exception e)
        {
            ArchipelagoMapMod.Instance.LogError($"Error cleaning up log manager: {e}");
        }

        logRequests = null;
        logConsumer = null;
        ModHooks.ApplicationQuitHook -= EnsureStopped;
    }

    public static void NewFile(string filename)
    {
        if (!ArchipelagoMapMod.Instance.GS.EnableTrackerLog)
        {
            return;
        }

        File.Create(Path.Combine(ApmmDirectory, filename)).Close();
    }

    public static void AppendLine(string filename, string line)
    {
        if (!ArchipelagoMapMod.Instance.GS.EnableTrackerLog)
        {
            return;
        }
        File.AppendAllText(Path.Combine(ApmmDirectory, filename), line);
    }
}
