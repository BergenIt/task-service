using System.Globalization;

using DatabaseExtension.Translator;

using TaskService.Core.Models;

namespace TaskService.Core.AuditWriter;

public class AuditWriter : IAuditWriter
{
    private const string ExecuteAction = "Execute";

    private readonly Grpc.Core.CallOptions _callOptions;

    private readonly IJwtParser _jwtParser;
    private readonly IAuditGrpcСlient _grpcСreator;
    private readonly ITranslator _translator;

    public AuditWriter(ITranslator translator, IJwtParser jwtParser, IAuditGrpcСlient auditGrpc)
    {
        _jwtParser = jwtParser;
        _grpcСreator = auditGrpc;
        _translator = translator;

        _callOptions = new();
    }

    public ValueTask UpsertJobRecord(AuditTaskAction action, TaskType taskType, string jobName, IDictionary<string, string> taskData)
    {
        string userName = _jwtParser.GetUserName();

        if (string.IsNullOrEmpty(userName))
        {
            return ValueTask.CompletedTask;
        }

        string ip = _jwtParser.GetIpAddress();

        string template = _translator.GetUserText<AuditWriter>(nameof(UpsertJobRecord));
        string jobUserName = _translator.GetUserText<Quartz.IJob>(jobName);
        string taskName = _translator.GetUserText<TaskType>(taskType.ToString());
        string message = string.Format(CultureInfo.InvariantCulture, template, jobUserName, taskName, DataToMessage(taskData));

        AuditCreateCommand auditCreateCommand = new(action.ToString(), message, ip, userName);

        return new(_grpcСreator.CreateAuditAsync(auditCreateCommand, _callOptions).ResponseAsync);
    }

    public ValueTask ExecuteJobRecord<TData>(string jobName, TData taskData)
    {
        string userName = _jwtParser.GetUserName();

        if (string.IsNullOrEmpty(userName))
        {
            return ValueTask.CompletedTask;
        }

        string ip = _jwtParser.GetIpAddress();

        string template = _translator.GetUserText<AuditWriter>(nameof(UpsertJobRecord));
        string jobUserName = _translator.GetUserText<Quartz.IJob>(jobName);
        string message = string.Format(CultureInfo.InvariantCulture, template, jobUserName, DataToMessage(taskData));

        AuditCreateCommand auditCreateCommand = new(ExecuteAction, message, ip, userName);

        return new(_grpcСreator.CreateAuditAsync(auditCreateCommand, _callOptions).ResponseAsync);
    }

    private static string DataToMessage(IDictionary<string, string> data)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    private static string DataToMessage<TData>(TData data)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }
}
