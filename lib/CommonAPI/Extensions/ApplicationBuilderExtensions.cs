using CommonAPI.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CommonAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder SetupDefaultApp<TClass>(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
            where TClass : class
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseMiddleware<OperationCancelledExceptionMiddleware>();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(TClass).Namespace);
            });

            // Dev note: Disabled, since the localhost dev ssl does not allow container to container communications
            // We could create our own ssl certificates.
            // app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}
