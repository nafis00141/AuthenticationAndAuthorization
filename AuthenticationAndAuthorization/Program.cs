using AuthenticationAndAuthorization.Infrastructure;
using AuthenticationAndAuthorization.Infrastructure.Swagger;
using AuthenticationAndAuthorizationInfrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.RegisterSwagger();
builder.Services.RegisterServices();
builder.Services.RegisterAuthenticationServices();

var app = builder.Build();

app.UseMiddleware<AuthenticationMiddleware>();


app.MapAuthenticationEndpoints();

app.UseSwaggerInApp();
app.UseHttpsRedirection();

app.Run();
