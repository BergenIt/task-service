namespace DatabaseExtension.Pagination;

/// <summary>
/// Запрос на пагинацию
/// </summary>
public record PaginationFilter(int? PageNumber, int? PageSize);
