namespace DatabaseExtension.TimeRange;

public record TimeRangeFilter(string ColumnName, DateTime StartRange, DateTime EndRange);
