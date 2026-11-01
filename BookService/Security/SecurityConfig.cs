using Library.Security;

namespace Library.BookService.Security
{
    public static class SecurityConfig
    {
        public static void ConfigureSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLibraryJwtAuthentication(configuration);

            // Solo se qualcuno usa la policy; altrimenti eliminala.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });
        }
    }
}