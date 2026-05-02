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

// 2. Configuração de CORS (Essencial para o Swagger não dar "Failed to Fetch")
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

// 3. Configuração do Refit (Comunicação entre Microserviços)
// Se estiver no VS, o Proposta.Api geralmente roda em http://localhost:5001
var propostaBaseUrl = builder.Configuration["PropostaService:BaseUrl"]
    ?? builder.Configuration["PropostaService__BaseUrl"]
    ?? "http://localhost:5001"; // Fallback para debug local no VS

// Substitua o trecho do builder.Services.AddRefitClient por este:
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

builder.Services.AddRefitClient<IPropostaServiceApi>()
    .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(propostaBaseUrl))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        // Força a aceitação de qualquer certificado para evitar o erro de SSL
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    });

// 4. Connection String (Prioriza appsettings, senão usa localhost para debug)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=db_plataforma_seguros;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ContratacaoDbContext>(options =>
    options.UseNpgsql(connectionString));

// 5. Injeção de Dependência
builder.Services.AddScoped<IPropostaServiceClient, PropostaServiceClient>();
builder.Services.AddScoped<IContratacaoRepository, ContratacaoRepository>();
builder.Services.AddScoped<ContratarPropostaUseCase>();

var app = builder.Build();

// 6. Auto-Migration no Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ContratacaoDbContext>();
        Log.Information("Aplicando migrations no PostgreSQL...");
        dbContext.Database.Migrate();
        Log.Information("Banco de dados sincronizado.");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Falha crítica ao iniciar o banco de dados");
        throw;
    }
}

// 7. Middleware e Pipeline
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Habilita o CORS
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// COMENTADO para evitar problemas de SSL local no Windows
// app.UseHttpsRedirection(); 

app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();