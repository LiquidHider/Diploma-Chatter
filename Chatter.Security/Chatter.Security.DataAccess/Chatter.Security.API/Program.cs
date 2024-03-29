using Chatter.Security.API.AssemblyMarker;
using Chatter.Security.API.Extensions;
using Chatter.Security.API.Interfaces;
using Chatter.Security.API.Mapping;
using Chatter.Security.API.Options;
using Chatter.Security.API.Services;
using Chatter.Security.Core.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidation();
builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
builder.Services.AddAutoMapper(typeof(CoreToResponseProfile));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddChatterCors(builder.Configuration);
builder.Services.AddSecurityCore(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsOptions.PolicyName);
app.UseAuthorization();

app.MapControllers();

app.Run();
