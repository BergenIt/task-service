
using TaskService.Core.TaskSystemImplementations.Http;

namespace TaskService.Core.SystemImplimentationType;

public enum ImplimentationType
{
    [ImplimentationType(typeof(HttpValidator), typeof(HttpSelector), typeof(HttpSender))]
    Http,

    [ImplimentationType(typeof(GrpcValidator), typeof(GrpcSelector), typeof(GrpcSender))]
    Grpc,

    //Добавить при необходимости (брокера для EventSoursing, скрипты для кастомных задач с ui)
    //Nats,
    //JavaScript,
    //Python,
    //CSharp,
    //Binary,
    //GraphQL
}
