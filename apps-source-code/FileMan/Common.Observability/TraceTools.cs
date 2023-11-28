using System.Diagnostics;

namespace Common.Observability;

public static class TraceTools
{
    public static ActivityTagsCollection GetTags()
    {
        static object? Var(string k) => Environment.GetEnvironmentVariable(k ?? string.Empty);
        return new()
        {
            ["namespace"] = Var("K8S_NAMESPACE"),
            ["app"] = Var("K8S_APP"),
            ["pod"] = Var("K8S_POD"),
        };
    }
}
