using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if(context.Request != null)
            {
                string method = context.Request.Method;
                string path = context.Request.Path;
                string bodyStr = "";
                string queryString = context.Request.QueryString.ToString();

                using(var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }
                string[] lines = { method, path, bodyStr, queryString};
                string dirPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
                string logPath = dirPath + "/requestsLog.txt";
                if (!File.Exists(logPath))
                    File.CreateText(logPath);
                using (var writer = File.AppendText(logPath))
                {
                    foreach (string s in lines)
                    {
                        if(!String.IsNullOrEmpty(s))
                            writer.WriteLine(s);
                    }
                }

            }
            if(_next != null) await _next(context);
        }
    }
}
