using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SimulacaoCredito.Application.Interfaces;
using SimulacaoCredito.Infrastructure.Data;
using SimulacaoCredito.Infrastructure.Services;
using SimulacaoCredito.Infrastructure.Middleware;
using SimulacaoCredito.Infrastructure.Security;
using SimulacaoCredito.Infrastructure.Repositories;
using SimulacaoCredito.Domain.Entities;
using System.Text;

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

// Para demonstração, usar banco em memória se SQL Server não estiver disponível
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("SimulacaoCredito_InMemory"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("LocalSqlServer")));
}

builder.Services.AddDbContext<ProdutoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IAmortizacaoService, AmortizacaoService>();
builder.Services.AddScoped<ISimulacaoService, SimulacaoService>();
builder.Services.AddScoped<IEventHubService, EventHubService>();
builder.Services.AddScoped<ITelemetriaService, TelemetriaService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();

// Configuração de autenticação JWT conforme especificado no documento de arquitetura
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "MinhaChaveSecretaSuperSeguraParaJWT2024!@#$%";
var key = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; // Em produção, definir como true
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

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

// Configuração de autenticação e autorização conforme documento de arquitetura
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Criar usuário de teste no banco em memória (apenas em desenvolvimento)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHashService = scope.ServiceProvider.GetRequiredService<IPasswordHashService>();

        // Garantir que o banco existe
        await context.Database.EnsureCreatedAsync();

        // Verificar se já existe usuário de teste
        if (!await context.Usuarios.AnyAsync())
        {
            var usuarioTeste = new Usuario
            {
                Username = "testuser",
                PasswordHash = passwordHashService.HashPassword("admin123"),
                Email = "testuser@simulacaocredito.com",
                NomeCompleto = "Usuário de Teste",
                Ativo = true,
                DataCriacao = DateTimeOffset.UtcNow
            };

            context.Usuarios.Add(usuarioTeste);
            await context.SaveChangesAsync();

            app.Logger.LogInformation("Usuário de teste criado: {Username}", usuarioTeste.Username);
        }
    }
}

app.Run();
