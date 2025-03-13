namespace MyDemo.Extensions
{
    public static class Middleware
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger()
                   .UseSwaggerUI();
            }

            app.UseCors("AllowSpecificOrigins")
               .UseHttpsRedirection()
               .UseAuthentication()
               .UseAuthorization();

            app.MapControllers();
        }
    }
}
