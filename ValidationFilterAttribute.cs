using System;

public class ValidationFilterAttribute : IActionFilter
{
    public ValidationFilterAttribute()
    {

    }
    public void OnActionExecuted(ActionExecutedContext context)
    {

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];

        var param = context.ActionArguments
            .SingleOrDefault(x => x.Value.ToString().Contains("Dto")).Value;
        if (param != null)
        {
            context.Result = new BadRequestObjectResult($"Object is null. Controller:{controller}, action: {action}");
        }
        if (!context.ModelState.IsValid)
        {
            context.Result = new UnprocessableEntityObjectResult(context.ModelState);
        }
    }
}

