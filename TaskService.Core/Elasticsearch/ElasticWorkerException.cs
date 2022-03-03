namespace TaskService.Core.Elasticsearch;

public class ElasticWorkerException : Exception
{
    public ElasticWorkerException(string message) : base(message) { }
}
