using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace HotelsApp.API.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            // Логирование ошибки (можно добавить использование LoggingService)
            var exception = context.Exception;
            var message = exception.Message;
            var innerException = exception.InnerException?.Message;

            // Определение статус кода
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            
            if (exception is ArgumentException || exception is ArgumentNullException)
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
            }
            else if (exception is System.Data.Entity.Core.EntityException)
            {
                statusCode = HttpStatusCode.ServiceUnavailable;
                message = "Ошибка подключения к базе данных";
            }

            // Формирование ответа
            var errorResponse = new
            {
                error = message,
                details = innerException,
                timestamp = DateTime.Now
            };

            var response = context.Request.CreateResponse(statusCode, errorResponse);
            context.Response = response;
        }
    }
}

