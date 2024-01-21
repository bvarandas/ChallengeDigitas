using Common.Logging.Correlation;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;

var builder = WebApplication.CreateBuilder(args);

IConfigurationBuilder configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

configuration.AddJsonFile($"ocelot.{environment.EnvironmentName}.json", true, true);

builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
                policy => {
                    policy
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();

                });
});
builder.Services.AddOcelot()
    .AddKubernetes()
    .AddCacheManager(o => o.WithDictionaryHandle());
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.AddCorrelationIdMiddleware();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello Ocelot"); });
});
app.UseWebSockets();
await app.UseOcelot();

app.Run();
