using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Infrastructure.Data;
using SimulacaoCredito.Infrastructure.Services;
using SimulacaoCredito.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Banco local para persistir simulações
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalSqlServer")));

// Banco externo para consultar produtos (somente leitura)
builder.Services.AddDbContext<ProdutoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// Registrar serviços
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IAmortizacaoService, AmortizacaoService>();
builder.Services.AddScoped<ISimulacaoService, SimulacaoService>();
builder.Services.AddScoped<IEventHubService, EventHubService>();
builder.Services.AddScoped<ITelemetriaService, TelemetriaService>();

// Controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware de telemetria
app.UseMiddleware<TelemetriaMiddleware>();

// Mapear controllers
app.MapControllers();

app.Run();
