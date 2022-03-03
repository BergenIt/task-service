namespace TaskService.Core.Elasticsearch;

public class ElasticMigrateException : Exception
{
    public ElasticMigrateException(string message) : base(message) { }
}
