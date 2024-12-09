using Microsoft.AspNetCore.Http;
using SEO.Optimize.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Web.Middlewares
{
    public class SessionValidatorMiddleware : IMiddleware
    {
        private readonly AppStateCache appStateCache;

        public SessionValidatorMiddleware(AppStateCache appStateCache)
        {
            this.appStateCache = appStateCache;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
