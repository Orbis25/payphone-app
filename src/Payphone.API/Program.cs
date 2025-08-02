using System.Reflection;
using Payphone.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
builder.Services.AddInfrastructure(builder.Configuration, xml);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/health", () => Results.Ok("Healthy"));
app.UseHttpsRedirection();

app.MapControllers();

app.UseInfrastructure();

app.Run();
