﻿using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Core.Utilities.Messages;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Core.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _ = e.Message;
            string message;
            message = e.Message; 

            if (e.GetType() == typeof(ValidationException))
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            else if (e.GetType() == typeof(ApplicationException))
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            else if (e.GetType() == typeof(UnauthorizedAccessException))
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            else if (e.GetType() == typeof(SecurityException))
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            else
                message = ExceptionMessage.InternalServerError;

            await httpContext.Response.WriteAsync(message);
        }
    }
}