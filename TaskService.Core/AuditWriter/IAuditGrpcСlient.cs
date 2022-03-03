
using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

namespace TaskService.Core.AuditWriter;

public interface IAuditGrpcСlient
{
    AsyncUnaryCall<Empty> CreateAuditAsync(AuditCreateCommand request, CallOptions options);
}
