using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EyeProtect.Manage.Configuration
{
    internal static class ApplicationBuilderExtenstions
    {
        /// <summary>
        /// 启用 SwaggerUi
        /// </summary>
        public static void UseSwaggerDashboard(this IApplicationBuilder app, IConfiguration config,
            string basePath, (string Name, string Title)[] docs)
        {
            // Api文档
            app.UseSwagger(c => { c.RouteTemplate = $"/{basePath}/swagger/doc/{{*documentName}}"; });
            app.UseSwaggerUI(c =>
            {
                //var authOptions = app.ApplicationServices.GetRequiredService<IOptions<CommonOAuthOptions>>().Value;

                //c.OAuthClientId(authOptions.ClientId);
                //c.OAuthClientSecret(authOptions.ClientSecret);
                //c.OAuthAppName(authOptions.ApiName);
                c.OAuthScopeSeparator(" ");

                c.RoutePrefix = $"{basePath}/swagger";

                foreach (var (name, title) in docs)
                {
                    c.SwaggerEndpoint($"/{basePath}/swagger/doc/{name}", title);
                }
            });
        }
    }
}
