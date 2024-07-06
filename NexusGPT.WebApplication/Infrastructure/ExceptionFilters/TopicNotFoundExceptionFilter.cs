using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NexusGPT.UseCase.Exceptions;

namespace NexusGPT.WebApplication.Infrastructure.ExceptionFilters;

[AttributeUsage(AttributeTargets.Method|AttributeTargets.Class)]
public class TopicNotFoundExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is TopicNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new
            {
                StatusCode = 404,
                Message = context.Exception.Message
            });
            context.ExceptionHandled = true;
        }
        
        base.OnException(context);
    }
}