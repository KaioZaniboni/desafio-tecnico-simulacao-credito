using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Infrastructure.Data;
using SimulacaoCredito.Infrastructure.Services;
using SimulacaoCredito.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

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
app.UseMiddleware<TelemetriaMiddleware>();
app.MapControllers();

app.Run();
