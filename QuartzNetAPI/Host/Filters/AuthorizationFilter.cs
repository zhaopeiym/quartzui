using Host.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
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
            var token = context.HttpContext.Request.Headers["token"].ToString();

            if (context.HttpContext.Request.Path.Value == "/api/Seting/VerifyLoginInfo")
            {
                return Task.CompletedTask;
            }

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
