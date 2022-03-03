using AutoMapper;

using Google.Protobuf.WellKnownTypes;

namespace TaskService.Automapper;

internal class TimeConverter : Profile
{
    public TimeConverter()
    {
        CreateMap<DateTime, Timestamp>().ConvertUsing(d => d.ToTimestamp());
        CreateMap<Timestamp, DateTime>().ConvertUsing(d => d.ToDateTime());

        CreateMap<TimeSpan, Duration>().ConvertUsing(t => t.ToDuration());
        CreateMap<Duration, TimeSpan>().ConvertUsing(t => t.ToTimeSpan());
    }
}
