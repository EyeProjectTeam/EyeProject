using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.CoreWeb.Filters
{
    public class HttpLogFilter : ActionFilterAttribute
    {
        /// <inheritdoc />
        public HttpLogFilter()
        {
            Order = -1000000;
        }

        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                context.HttpContext.Items["ActionDescriptor"] = context.ActionDescriptor;
            }
        }

        /// <inheritdoc />
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Items["ActionExecutedContext"] = context;
        }
    }
}
