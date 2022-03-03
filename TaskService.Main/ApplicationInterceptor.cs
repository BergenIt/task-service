
using Grpc.Core;
using Grpc.Core.Interceptors;

using Microsoft.Net.Http.Headers;

namespace TaskService;

/// <summary>
/// Interceptor для grpc подключений
/// </summary>
public class ApplicationInterceptor : Interceptor
{
    /// <summary>
    /// Обработка вызовов клиент-сервер grpc
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <param name="continuation"></param>
    /// <returns></returns>
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        Serilog.Log.Logger.ForContext(context.Host, context.Method).Debug("Start request with data {request}", request);

        string token = context.RequestHeaders.Get(HeaderNames.Authorization.ToLowerInvariant()).Value;

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, string.Empty));
        }

        try
        {
            TResponse response = await continuation(request, context);

            Serilog.Log.Logger.ForContext(context.Host, context.Method).Debug("Response to request with {response}", response);

            return response;
        }
        catch (Exception exception)
        {
            Serilog.Log.Logger.ForContext<Interceptor>().Error("Exception while runnig unary server handler: {exception}", exception);

            throw new RpcException(new(StatusCode.Unknown, GetExceptionMessage(exception)));
        }
    }

    private static string GetExceptionMessage(Exception exception)
    {
        Exception inner = exception;

        string message = $"{exception.GetType().Name}: {exception.Message}";

        ushort i = 0;

        while (inner.InnerException is not null && i < 5)
        {
            inner = inner.InnerException;

            message += $"\n{inner.GetType().Name}: {inner.Message}";

            ++i;
        }

        return message;
    }
}
