using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NexusGPT.UseCase.Exceptions;

namespace NexusGPT.WebApplication.Infrastructure.ExceptionFilters;

[AttributeUsage(AttributeTargets.Method|AttributeTargets.Class)]
public class MessageChannelNotFoundExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is MessageChannelNotFoundException)
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