using Host.Attributes;
using Host.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Talk.Extensions;

namespace Host.Filters
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var noNeedLoginAttribute = controllerActionDescriptor.
                                   ControllerTypeInfo.
                                   GetCustomAttributes(true)
                                   .Where(a => a.GetType().Equals(typeof(NoLoginAttribute)))
                                   .ToList();
                noNeedLoginAttribute.AddRange(controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                                 .Where(a => a.GetType().Equals(typeof(NoLoginAttribute))).ToList());

                //如果标记了 NoLoginAttribute 则不验证其登录状态
                if (noNeedLoginAttribute.Any())
                {
                    return Task.CompletedTask;
                }
            }

            var token = context.HttpContext.Request.Headers["token"].ToString();
            if (!token.IsNullOrWhiteSpace())
            {
                var time = token.DES3Decrypt().ToDateTime();
                //登录信息有效期为当天
                if (DateTime.Now.Date == time.Date)
                {
                    return Task.CompletedTask;
                }
            }

            context.Result = new JsonResult(new
            {
                ErrorMsg = "请登录",
                ResultUrl = "/signin",
            });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }
    }
}
