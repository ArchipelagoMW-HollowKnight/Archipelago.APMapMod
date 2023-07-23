﻿using System.Reflection;

namespace ArchipelagoMapMod;

internal static class Interop
{
    private static readonly Dictionary<string, Assembly> interopMods = new()
    {
        {"BenchRando", null},
        {"Benchwarp", null}
    };

    internal static void FindInteropMods()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            if (interopMods.ContainsKey(assembly.GetName().Name))
                interopMods[assembly.GetName().Name] = assembly;
    }

    internal static bool HasBenchRando()
    {
        return interopMods["BenchRando"] is not null;
    }

    internal static bool HasBenchwarp()
    {
        return interopMods["Benchwarp"] is not null;
    }
}