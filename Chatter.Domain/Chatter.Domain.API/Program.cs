using Chatter.Domain.API;
using Chatter.Domain.API.AssemblyMarker;
using Chatter.Domain.API.Extensions;
using Chatter.Domain.API.Mapping;
using Chatter.Domain.API.Options;
using Chatter.Domain.BusinessLogic.Extensions;
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
builder.Services.AddAutoMapper(typeof(DomainToResponseProfile));
builder.Services.AddBusinessLogic(builder.Configuration);
builder.Services.AddChatterCors(builder.Configuration);
builder.Services.AddChatterAuth(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(CorsOptions.PolicyName);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization(AuthOptions.DefaultAuthPolicy);

app.Run();
