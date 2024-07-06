using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NexusGPT.UseCase.Exceptions;

namespace NexusGPT.WebApplication.Infrastructure.ExceptionFilters;

[AttributeUsage(AttributeTargets.Method|AttributeTargets.Class)]
public class MessageChannelMaxCountExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is TopicMaxCountException)
        {
            context.Result = new BadRequestObjectResult(new
            {
                StatusCode = 400,
                Message = context.Exception.Message
            });
            context.ExceptionHandled = true;
        }
        
        base.OnException(context);
    }
}