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

// 2. Configuração de CORS (Essencial para evitar o "Failed to Fetch" no Swagger)
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

// 3. Connection String (Prioriza appsettings, senão usa localhost para debug)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=db_plataforma_seguros;Username=postgres;Password=postgres";

builder.Services.AddDbContext<PropostaDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.MigrationsAssembly("Proposta.Infrastructure")));

// 4. Injeção de Dependência (Arquitetura Hexagonal)
builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<PropostaUseCases>();

var app = builder.Build();

// 5. Pipeline de Middlewares
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Habilita o CORS antes de outros middlewares de rota
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. Migrations Automáticas ao Iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PropostaDbContext>();
    try
    {
        Console.WriteLine("Aplicando migrations Proposta...");
        // Isso garante que as tabelas e o Seed Data sejam criados no banco do Docker
        dbContext.Database.Migrate();
        Console.WriteLine("Migrations Proposta aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao aplicar migrations: {ex.Message}");
        throw;
    }
}

// COMENTADO para evitar problemas de certificado local no Windows/Docker
// app.UseHttpsRedirection(); 

app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();