using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace SIDPSF.Common.RequestFilters
{
    public static class RequestLocalizationCookiesMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLocalizationCookies(this IApplicationBuilder app)
        {

            app.UseMiddleware<RequestLocalizationCookiesMiddleware>();
            return app;
        }
    }

    public class RequestLocalizationCookiesMiddleware : IMiddleware
    {
        public CookieRequestCultureProvider Provider { get; }

        public RequestLocalizationCookiesMiddleware(IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            Provider =
                requestLocalizationOptions
                    .Value
                    .RequestCultureProviders
                    .Where(x => x is CookieRequestCultureProvider)
                    .Cast<CookieRequestCultureProvider>()
                    .FirstOrDefault();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (Provider != null)
            {

                var feature = context.Features.Get<IRequestCultureFeature>();

                if (feature != null)
                {
                    // remember culture across request
                    context.Response
                        .Cookies
                        .Append(
                            Provider.CookieName,
                            CookieRequestCultureProvider.MakeCookieValue(feature.RequestCulture)
                        );
                }
            }

            await next(context);
        }
    }

    public class CustomRequestCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            HttpContextAccessor _accessor = new HttpContextAccessor();

            string IdLanguage = _accessor.HttpContext.Session.GetString("IdLanguage");

            // SET INITIAL LANGUAGE ON FIRST LOAD //
            if (IdLanguage == "" || IdLanguage == null)
            {
                _accessor.HttpContext.Session.SetString("IdLanguage", "1");
                IdLanguage = _accessor.HttpContext.Session.GetString("IdLanguage");
            }

            switch (IdLanguage)
            {
                case "1":
                    await Task.Yield();
                    return new ProviderCultureResult("en-GB");

                case "2":
                    await Task.Yield();
                    return new ProviderCultureResult("sq-AL");

                case "3":
                    await Task.Yield();
                    return new ProviderCultureResult("sr-Latn-RS");

                default:
                    await Task.Yield();
                    return new ProviderCultureResult("en-GB");
            }
        }
    }
}