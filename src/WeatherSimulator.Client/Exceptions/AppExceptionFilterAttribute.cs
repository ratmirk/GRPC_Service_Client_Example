using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WeatherSimulator.Client.Exceptions;

public class AppExceptionFilterAttribute: ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ArgumentException ex:
                HandleEx(context, ex.Message, HttpStatusCode.BadRequest);
                return;

            case KeyNotFoundException ex:
                HandleEx(context, ex.Message, HttpStatusCode.NotFound);
                return;

            case { } ex:
                HandleEx(context, "Возникла ошибка. Обратитесь к разработчикам", HttpStatusCode.InternalServerError);
                return;

        }
        base.OnException(context);
    }


    private void HandleEx(ExceptionContext context, string Message, HttpStatusCode code)
    {
        context.Result = new JsonResult(new { Error = Message })
        {
            StatusCode = (int)code
        };
    }
}