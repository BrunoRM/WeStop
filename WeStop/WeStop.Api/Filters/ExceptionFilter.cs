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
                    ((ErrorException)context.Exception).Error
                });
            }
            else if (context.Exception is ValidationException)
            {
                context.Result = new OkObjectResult(new
                {
                    Ok = false,
                    ((ValidationException)context.Exception).Error,
                    Errors = ((ValidationException)context.Exception).Failures
                });
            }
            else if (context.Exception is NotFoundException)
            {
                context.Result = new OkObjectResult(new
                {
                    Ok = false,
                    ((NotFoundException)context.Exception).Error
                });
            }
            else
            {
                context.Result = new OkObjectResult(new
                {
                    Ok = false,
                    Error = "Ocorreu um erro interno não tratado"
                });
            }
        }
    }
}