using System.Diagnostics;
using System.Reflection;
using MethodBoundaryAspect.Fody.Attributes;

namespace Common.Observability;

public sealed class TraceThisAttribute : OnMethodBoundaryAspect, IDisposable
{
    private static readonly ActivityTagsCollection Tags = TraceTools.GetTags();
    private Activity? _activity;

    public override void OnEntry(MethodExecutionArgs args)
    {
        var assembly = Assembly.GetEntryAssembly()!.GetName();
        var source = new ActivitySource(assembly.Name!, assembly.Version?.ToString(3));
        var activityName = GetActivityName(args.Method);
        _activity = source.StartActivity(activityName, ActivityKind.Internal, null!, Tags);

        Debug.WriteLine($"Activity on {source.Name}; {activityName}");
    }

    public override void OnExit(MethodExecutionArgs args)
    {
        if (args.ReturnValue is Task task)
        {
            task.ContinueWith(_ => Dispose());
        }
        else
        {
            Dispose();
        }
    }

    public override void OnException(MethodExecutionArgs args)
    {
        var ex = args.Exception;
        var tags = new ActivityTagsCollection
        {
            ["type"] = ex.GetType().Name,
            ["message"] = ex.Message,
        };
        
        if (ex.InnerException != null)
        {
            tags.Add("innerType", ex.InnerException.GetType().Name);
            tags.Add("innerMessage", ex.InnerException.Message);
        }

        _activity?.AddEvent(new("exception", tags: tags));
        Dispose();
    }

    public void Dispose()
    {
        _activity?.Stop();
        _activity?.Source?.Dispose();
    }

    private static string GetActivityName(MethodBase method)
    {
        var declaringAssembly = Assembly.GetAssembly(method.DeclaringType!);
        var prefix = declaringAssembly?.GetName()?.Name;
        return $"[{prefix}] {method.DeclaringType?.Name}::{method.Name}()";
    }
}