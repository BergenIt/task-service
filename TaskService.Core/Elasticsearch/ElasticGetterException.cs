namespace TaskService.Core.Elasticsearch;

public class ElasticGetterException : Exception
{
    public ElasticGetterException(string message) : base(message) { }
}
