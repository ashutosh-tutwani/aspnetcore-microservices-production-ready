using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

Startup startup = new(builder);
// Add services to the container.
//startup.ConfigureServices(builder.Services);

var app = builder.Build();

//startup.Configure(app, app.Environment);

app.Run();
