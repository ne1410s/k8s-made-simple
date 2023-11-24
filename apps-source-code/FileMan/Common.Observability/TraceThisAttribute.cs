using System.Diagnostics;
using System.Reflection;
using MethodBoundaryAspect.Fody.Attributes;

namespace Common.Observability;

public sealed class TraceThisAttribute : OnMethodBoundaryAspect, IDisposable
{
    private readonly ActivitySource _activitySource;

    public string NamespaceVar { get; set; } = "K8S_NAMESPACE";

    public string AppVar { get; set; } = "K8S_APP";

    public string PodVar { get; set; } = "K8S_POD";

    public TraceThisAttribute()
    {
        var assembly = Assembly.GetCallingAssembly().GetName();
        _activitySource = new(assembly.Name!, assembly.Version?.ToString(3));
    }

    public override void OnEntry(MethodExecutionArgs args)
    {
        var activityName = GetActivityName(args.Method);
        var kind = ActivityKind.Internal;
        var tags = TraceTools.GetTags(NamespaceVar, AppVar, PodVar);
        using var activity = _activitySource.StartActivity(activityName, kind, null!, tags);

        Console.WriteLine($"Activity: {_activitySource.Name} / {activity}");
    }

    private static string GetActivityName(MethodBase method)
    {
        return $"{method.DeclaringType?.Name}::{method.Name}()";
    }

    public void Dispose()
    {
        _activitySource?.Dispose();
    }
}