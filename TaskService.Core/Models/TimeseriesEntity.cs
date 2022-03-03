using System.ComponentModel.DataAnnotations;

using Nest;

namespace TaskService.Core.Models;

[ElasticsearchType(IdProperty = nameof(Id))]
public abstract record TimeseriesEntity([property: Key][property: Ignore] string Id)
{
    [Ignore]
    public abstract string IndexKey { get; }
}
