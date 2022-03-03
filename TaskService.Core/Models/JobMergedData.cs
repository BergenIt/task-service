namespace TaskService.Core.Models;

public record JobMergedData(
    string Data,
    string SenderRoute,
    string? ValidatorRoute = null,
    string? SelectorRoute = null
);
