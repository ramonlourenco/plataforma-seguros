using System.Text.Json.Serialization;
using Contratacao.Application.UseCases;
using Contratacao.Domain.Ports;
using Contratacao.Infrastructure;
using Contratacao.Infrastructure.Clients;
using Contratacao.Infrastructure.Repositories;
using Contratacao.Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Refit;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Contratacao.Api")
    .WriteTo.Console(new CompactJsonFormatter()));

// 1. Configuração de Controllers e Enums
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

// 3. Configuração do Refit
var propostaBaseUrl = builder.Configuration["PropostaService:BaseUrl"]
    ?? builder.Configuration["PropostaService__BaseUrl"]
    ?? "http://localhost:5001";

builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

builder.Services.AddRefitClient<IPropostaServiceApi>()
    .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(propostaBaseUrl))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    });

// 4. Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=db_plataforma_seguros;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ContratacaoDbContext>(options =>
    options.UseNpgsql(connectionString));

// 5. Injeção de Dependência
builder.Services.AddScoped<IPropostaServiceClient, PropostaServiceClient>();
builder.Services.AddScoped<IContratacaoRepository, ContratacaoRepository>();
builder.Services.AddScoped<ContratarPropostaUseCase>();

var app = builder.Build();

// 6. Auto-Migration no Startup com Lógica de Retry (TENTATIVAS)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ContratacaoDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    int maxRetries = 5;
    int delay = 3000; // 3 segundos entre tentativas

    for (int i = 1; i <= maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Tentativa {Tentativa}/{Max} de aplicar migrations no PostgreSQL...", i, maxRetries);
            dbContext.Database.Migrate();
            logger.LogInformation("Banco de dados sincronizado com sucesso!");
            break;
        }
        catch (Exception ex)
        {
            if (i == maxRetries)
            {
                logger.LogCritical(ex, "Falha crítica após {Max} tentativas. O banco de dados não respondeu.", maxRetries);
                throw;
            }

            logger.LogWarning("Banco de dados ainda não está pronto. Nova tentativa em {Delay}ms...", delay);
            Thread.Sleep(delay);
        }
    }
}

// 7. Middleware e Pipeline
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();