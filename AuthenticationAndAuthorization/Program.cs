using AuthenticationAndAuthorization.Infrastructure;
using AuthenticationAndAuthorization.Infrastructure.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.RegisterSwagger();
builder.Services.RegisterServices();

builder.Services.AddAuthentication(AuthenticationSchemes.DefaultAuthenticationScheme).AddCookie(AuthenticationSchemes.CookieAuthenticationScheme);


var app = builder.Build();

app.UseAuthentication();

app.MapAuthenticationEndpoints();

app.UseSwaggerInApp();
app.UseHttpsRedirection();

app.Run();
