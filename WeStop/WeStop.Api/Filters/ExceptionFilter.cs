using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeStop.Application.Exceptions;

namespace WeStop.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ErrorException)
            {
                context.Result = new OkObjectResult(new
                {
                    Ok = false,
                    Error = ((ErrorException) context.Exception).Error
                });
            }
        }
    }
}