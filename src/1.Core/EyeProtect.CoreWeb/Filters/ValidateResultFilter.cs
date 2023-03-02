using EyeProject.Core.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace EyeProtect.CoreWeb.Filters
{
    public class ValidateResultFilter : ActionFilterAttribute, ITransientDependency
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var result = new ErrorResult(new SerializableError(context.ModelState), ResultCode.InvalidData);
                context.Result = new ObjectResult(result);
            }
        }
    }
}
