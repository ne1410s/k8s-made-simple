namespace Common.Observability;

public static class TraceTools
{
    public static Dictionary<string, object?> GetTags(
        string namespaceVar = "K8S_NAMESPACE",
        string appVar = "K8S_APP",
        string podVar = "K8S_POD")
    {
        static object? Var(string k) => Environment.GetEnvironmentVariable(k ?? string.Empty);
        return new Dictionary<string, object?>
        {
            ["namespace"] = Var(namespaceVar),
            ["app"] = Var(appVar),
            ["pod"] = Var(podVar),
        };
    }
}
