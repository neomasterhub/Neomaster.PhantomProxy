using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Neomaster.PhantomProxy.App;
using Neomaster.PhantomProxy.Infra;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  var xmlFile = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  options.IncludeXmlComments(xmlPath);
});

// Phantom Proxy configuration
builder.Services.AddPhantomProxy(builder.Configuration);
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IProxyService, ProxyService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IUrlEncryptService, UrlAesEncryptService>();
builder.Services.AddMemoryCache();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
  options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
  options.KnownNetworks.Clear();
  options.KnownProxies.Clear();
});

var app = builder.Build();
app.UseForwardedHeaders();
app.Use(async (context, next) =>
{
  context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
  context.Response.Headers.Pragma = "no-cache";
  context.Response.Headers.Expires = "0";
  await next();
});
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
