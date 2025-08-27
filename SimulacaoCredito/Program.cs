using Microsoft.EntityFrameworkCore;
using Serilog;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Infrastructure.Data;
using SimulacaoCredito.Infrastructure.Services;
using SimulacaoCredito.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog conforme especificado no documento de arquitetura
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("logs/simulacao-credito-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj} {Properties:j}{NewLine}{Exception}")
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "SimulacaoCredito")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalSqlServer")));

builder.Services.AddDbContext<ProdutoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IAmortizacaoService, AmortizacaoService>();
builder.Services.AddScoped<ISimulacaoService, SimulacaoService>();
builder.Services.AddScoped<IEventHubService, EventHubService>();
builder.Services.AddScoped<ITelemetriaService, TelemetriaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Exception Handler global conforme especificado no documento de arquitetura
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseMiddleware<TelemetriaMiddleware>();

app.MapControllers();

app.Run();
