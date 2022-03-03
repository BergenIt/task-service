using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using TaskService.Core.AuditWriter;

namespace UserService.Proto;

public partial class AuditService
{
    public partial class AuditServiceClient : IAuditGrpcСlient
    {
        /// <summary>
        /// Метод расширения, используемый в Core проекте
        /// </summary>
        /// <param name="request"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public AsyncUnaryCall<Empty> CreateAuditAsync(TaskService.Core.AuditWriter.AuditCreateCommand request, CallOptions options)
        {
            AuditCreateCommand auditCreateCommand = new()
            {
                Action = request.Action,
                IpAddress = request.IpAddress,
                Message = request.Message,
                UserName = request.UserName
            };

            return CreateAuditAsync(auditCreateCommand, options);
        }
    }
}
