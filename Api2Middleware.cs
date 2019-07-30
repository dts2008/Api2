using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class Api2Middleware
    {
        private readonly RequestDelegate _next;

        public Api2Middleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string[] options = context.Request.Path.Value.Split('/');
            if (options.Length < 2) return;

            string contract = options[options.Length - 2].ToLower();
            string cmd = options[options.Length - 1].ToLower();

            //var token = context.Request.Query["token"];
            if (string.Compare(contract, "api2", true) == 0)
            {
                await ProcessApi2(context, cmd);
                //context.Response.StatusCode = 403;
                //await context.Response.WriteAsync("Token is invalid");
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        public async Task ProcessApi2(HttpContext context, string cmd)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (Api2Engine.Instance.IsApi(cmd))
            {
                await Api2Engine.Instance.Process(context, cmd);
                return;
            }
        }
    }
}
