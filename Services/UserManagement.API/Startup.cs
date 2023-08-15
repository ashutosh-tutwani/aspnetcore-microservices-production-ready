using Amazon.Runtime;
using AWS.Logger;
using AWS.Logger.SeriLog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Serilog;
using Serilog.Formatting.Compact;
using Services.Common;
using Services.Common.Extensions;
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
        services.AddSwagger(xmlPath);

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
        // create serilog logger
        Log.Logger = CreateSerilogLogger(env);
        Log.Logger.Information("log configured");

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

        app.UseAuthentication();
        app.UseAuthorization();

        // Configure the HTTP request pipeline.
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/user-management/swagger/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/user-management/swagger/v1/swagger.json", "User Management API");
            c.RoutePrefix = "api/user-management/swagger";
        });

        app.MapControllers().RequireAuthorization();
    }

    #region Serilog

    private Serilog.ILogger CreateSerilogLogger(IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            return SerilogLoggerConfig()
                    .ReadFrom.Configuration(Configuration)
                    .WriteTo.File(new RenderedCompactJsonFormatter(), Path.Combine(env.ContentRootPath, "Logs", "UserManagement.API.json"),
                    rollingInterval: RollingInterval.Day, retainedFileCountLimit: 5)
                    .WriteTo.Console()
                    .CreateLogger();
        }

        var credentials = new BasicAWSCredentials(Configuration.GetValue<string>("AWS_ACCESS_KEY_ID"),
                                                  Configuration.GetValue<string>("AWS_SECRET_ACCESS_KEY"));

        // create a logger for AWS Cloudwatch
        var awsConfiguration = new AWSLoggerConfig
        {
            Credentials = credentials,
            LogGroup = "/logs/merchant-portal-api",
            Region = ""
        };

        return SerilogLoggerConfig()
                 .ReadFrom.Configuration(Configuration)
                  .WriteTo.AWSSeriLog(awsConfiguration, textFormatter: new RenderedCompactJsonFormatter())
                  .CreateLogger();
    }

    private static LoggerConfiguration SerilogLoggerConfig()
    {
        return new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithDatadogTraceIdAndSpanId()
            .Enrich.WithProperty("service.instance.id", ServiceDetails.InstanceId);
    }

    #endregion Serilog
}