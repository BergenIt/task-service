using System.Globalization;

using Microsoft.Extensions.Configuration;

using TaskService.Core.Models;

namespace TaskService.Core.Options;

public class ProjectOptions : ProjectOptionsLogic
{
    public ProjectOptions(IConfiguration configuration) : base(configuration) { }

    public WriteLevel WriteLevel => GetVariable<WriteLevel>("Jobs:WriteLevel");

    public string ShadowJobsYaml => GetVariable("Jobs:Shadow");
    public string SystemJobsYaml => GetVariable("Jobs:System");
    public string UserJobsYaml => GetVariable("Jobs:User");

    public Uri UserServiceUri => new(GetVariable("UserService:Uri"));

    public Uri ElasticsearchUri => new(GetVariable("Elasticsearch:Uri"));
    public string ElasticsearchToken => GetVariable("Elasticsearch:BasicToken", validationIgnore: true);

    public string PsqlDbConnectionString => $"Host={PsqlDbHost};{(PsqlDbPort == default ? string.Empty : $"Port={PsqlDbPort};")}User Id={PsqlDbUser};Password={PsqlDbPass};Database={PsqlDbName};";
    public string PsqlDbConnectionStringWithoutDbName => $"Host={PsqlDbHost};{(PsqlDbPort == default ? string.Empty : $"Port={PsqlDbPort};")}User Id={PsqlDbUser};Password={PsqlDbPass};";

    public string PsqlDbHost => GetVariable("Psql:Host");
    public int PsqlDbPort => GetVariable<int>("Psql:Port", true);
    public string PsqlDbName => GetVariable("Psql:Database");
    public string PsqlDbUser => GetVariable("Psql:User");
    public string PsqlDbPass => GetVariable("Psql:Password");

    public string PsqlMigrateSqlFile => GetVariable("Psql:SqlFile");

    public int HttpPort => int.Parse(GetVariable("Kestrel:EndPoints:Http1:Url").Split(":").Last(), CultureInfo.InvariantCulture);
    public int GrpcPort => int.Parse(GetVariable("Kestrel:EndPoints:Http2:Url").Split(":").Last(), CultureInfo.InvariantCulture);

    public IEnumerable<string> TranslatorFiles => GetVariables("Translator");
}
