using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequestAuthorisationNoCheckFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null)
            return;
        else
        {
            // Set cache control headers to prevent caching
            context.HttpContext.Response.Headers["Cache-Control"] = "no-store, must-revalidate";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";

            var session = context.HttpContext.Session;

            // Generate a GUID for the request
            string requestGuid = Guid.NewGuid().ToString();
            session.SetString("GlobalAccessTrackingGUID", requestGuid);
        }
    }
}
