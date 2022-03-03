using System.Linq.Expressions;

using AutoMapper;

namespace TaskService.Automapper;

internal static class MapperTaskNameConvertor
{
    public static IMappingExpression<TSource, TDestination> ForMemberTranslateOrSource<TSource, TDestination>(this
            IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, string>> destinationMember,
            Expression<Func<TSource, string>> sourceMember
        )
    {
        return mappingExpression.ForMember(
            destinationMember,
            o => o.ConvertUsing<TaskNameOrSourseConvertor, string>(sourceMember)
        );
    }

    public static IMappingExpression<TSource, TDestination> ForMemberTranslate<TSource, TDestination>(this
            IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, string>> destinationMember,
            Expression<Func<TSource, string>> sourceMember
        )
    {
        return mappingExpression.ForMember(
            destinationMember,
            o => o.ConvertUsing<TaskNameConvertor, string>(sourceMember)
        );
    }

    public static IMappingExpression<TSource, TDestination> ForMemberRetranslate<TSource, TDestination>(this
            IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, string>> destinationMember,
            Expression<Func<TSource, string>> sourceMember
        )
    {
        return mappingExpression.ForMember(
            destinationMember,
            o => o.ConvertUsing<ReverseTaskNameConvertor, string>(sourceMember)
        );
    }
}
