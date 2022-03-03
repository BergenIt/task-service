using System.Reflection;

using Grpc.Core;
using Grpc.Net.Client;

using TaskService.Core.TaskExecutor;

namespace TaskService.Core.TaskSystemImplementations.Http;

//To di as singleton
public class GrpcRouteParser : IRouteParser<GrpcRoute>
{
    private const string Async = "Async";

    private readonly Dictionary<string, (Type, Dictionary<string, MethodInfo>)> _services;

    public GrpcRouteParser(IEnumerable<Type> services)
    {
        _services = services
            .ToDictionary(
                k => k.DeclaringType?.Name ?? throw new KeyNotFoundException(k.FullName),
                v => (v, v
                    .GetMethods()
                    .Where(m =>
                        !m.Name.Contains(Async)
                        && m.GetParameters().Any(p => p.ParameterType == typeof(CallOptions)))
                    .ToDictionary(k => k.Name)
                )
            );
    }

    public GrpcRoute Parse(string route)
    {
        GrpcRouteYamlSchema grpcRouteYamlSchema = Newtonsoft.Json.JsonConvert.DeserializeObject<GrpcRouteYamlSchema>(route)
            ?? throw new InvalidCastException($"Not parse yaml from: {route}");

        (Type serviceType, IDictionary<string, MethodInfo> methods) = _services[grpcRouteYamlSchema.Service];

        MethodInfo methodInfo = methods[grpcRouteYamlSchema.Method];

        return new(GrpcChannel.ForAddress(grpcRouteYamlSchema.Uri), serviceType, methodInfo);
    }
}

