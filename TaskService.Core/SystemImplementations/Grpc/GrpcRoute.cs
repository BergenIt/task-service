using System.Reflection;

using Grpc.Net.Client;

namespace TaskService.Core.TaskSystemImplementations.Http;

public record GrpcRoute(GrpcChannel Channel, Type ServiceCleintType, MethodInfo MethodType) : IDisposable
{
    public void Dispose()
    {
        Channel.Dispose();

        GC.SuppressFinalize(this);
    }
}

