using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Proposta.Application.UseCases;
using Proposta.Domain.Ports;
using Proposta.Infrastructure;
using Proposta.Api.Middleware;
using Proposta.Infrastructure.Repositories;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Proposta.Api")
    .WriteTo.Console(new CompactJsonFormatter()));

// 1. Configuração de Controllers e Enums no JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// 2. Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// 3. Connection String (Prioriza variáveis do Docker-Compose)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=db_plataforma_seguros;Username=postgres;Password=postgres";

builder.Services.AddDbContext<PropostaDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.MigrationsAssembly("Proposta.Infrastructure")));

// 4. Injeção de Dependência
builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<PropostaUseCases>();

var app = builder.Build();

// 5. Pipeline de Middlewares
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. Migrations Automáticas com Retry (Lógica para aguardar o Docker)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<PropostaDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    int maxRetries = 5;
    int delay = 3000; // 3 segundos entre tentativas

    for (int i = 1; i <= maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Tentativa {Tentativa}/{Max} de aplicar migrations (Proposta)...", i, maxRetries);
            dbContext.Database.Migrate();
            logger.LogInformation("Migrations de Proposta aplicadas com sucesso!");
            break;
        }
        catch (Exception ex)
        {
            if (i == maxRetries)
            {
                logger.LogCritical(ex, "Erro fatal: Banco de dados não disponível após {Max} tentativas.", maxRetries);
                throw;
            }

            logger.LogWarning("Banco ainda não aceita conexões. Nova tentativa em {Delay}ms...", delay);
            Thread.Sleep(delay);
        }
    }
}

app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();