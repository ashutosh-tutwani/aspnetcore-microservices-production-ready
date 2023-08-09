using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Reflection;

public class Startup
{
    private WebApplicationBuilder builder;
    private ConfigurationManager Configuration;

    public Startup(WebApplicationBuilder builder)
    {
        this.builder = builder;
        Configuration = builder.Configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        services.AddCors(options =>
        {
            options.AddPolicy("crs", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            });
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
            config.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors("crs");

        app.UseAuthentication();
        app.UseAuthorization();

        // Configure the HTTP request pipeline.
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/portal/swagger/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/portal/swagger/v1/swagger.json", "Merchant Portal API");
            c.RoutePrefix = "api/portal/swagger";
        });

        app.MapControllers().RequireAuthorization();
    }
}
