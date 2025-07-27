using System.Reflection;
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
builder.Services.AddPhantomProxy();

var app = builder.Build();
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
