using Conditionals.Core.Areas.Events;

namespace Conditionals.Core.Common.Utilities;

internal static class GeneralUtils
{
    public static IReadOnlyList<(string assemblyQualifiedName, string fullName)> AssemblyTypeNames { get; } 
    public static IReadOnlyList<(string assemblyQualifiedName, string fullName)> EventTypeNames    { get; } 


    static GeneralUtils()
    {
        string[] excludedNamespaces = ["System.", "Microsoft."];

        var filteredAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !excludedNamespaces.Any(exclude => a.GetName().Name!.StartsWith(exclude))).ToList();

        AssemblyTypeNames = filteredAssemblies.SelectMany(assembly => assembly.GetTypes().Select(t => (t.AssemblyQualifiedName, t.FullName)))
                                              .Where(t => !excludedNamespaces.Any(exclude => t.AssemblyQualifiedName!.StartsWith(exclude)))
                                              .ToList()!;

        EventTypeNames = filteredAssemblies.SelectMany(assembly => assembly.GetTypes())
                                           .Where(t => (t.BaseType != null && t.BaseType.IsGenericType && (t.BaseType.GetGenericTypeDefinition() == typeof(ConditionEventBase<>) || t.BaseType.GetGenericTypeDefinition() == typeof(RuleEventBase<>))))
                                           .Select(t => (t.AssemblyQualifiedName, t.FullName))
                                           .ToList()!;
    }
}
