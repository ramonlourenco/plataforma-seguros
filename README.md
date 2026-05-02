# PlataformaSeguros

Plataforma de Seguros - Solução .NET 8 com arquitetura Hexagonal para dois microserviços:

- `PropostaService`: CRUD de propostas com status `EmAnalise`, `Aprovada`, `Rejeitada`.
- `ContratacaoService`: efetua contratação apenas se a proposta estiver `Aprovada`.

## Estrutura

```
PlataformaSeguros/
├── Contratacao.Api/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Program.cs
│   └── Dockerfile
├── Contratacao.Application/
│   └── UseCases/
├── Contratacao.Domain/
│   ├── Entities/
│   ├── Ports/
│   └── ValueObjects/
├── Contratacao.Infrastructure/
│   ├── Clients/
│   └── Repositories/
├── Contratacao.Tests/
├── Proposta.Api/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Migrations/
│   ├── Program.cs
│   └── Dockerfile
├── Proposta.Application/
│   └── UseCases/
├── Proposta.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Ports/
├── Proposta.Infrastructure/
│   ├── Migrations/
│   └── Repositories/
├── Proposta.Tests/
├── docker-compose.yml
├── docker-compose.db.yml
└── README.md
```

- `*.Api`: camadas API/Controllers.
- `*.Application`: casos de uso e regras de aplicação.
- `*.Domain`: entidades e portas (interfaces).
- `*.Infrastructure`: adapters, repositório e cliente HTTP.
- `*.Tests`: testes unitários xUnit + Moq.

## Como rodar

### Full Stack
```bash
docker-compose up -d --build
```

### Apenas Banco (Debug VS)
```bash
docker-compose -f docker-compose.db.yml up -d
```

### Reset Ambiente
```bash
docker-compose down -v
```

O `PropostaService` aplica migrações automaticamente ao iniciar e cria o banco PostgreSQL.

### Rodar via Visual Studio Community

1. Abra `PlataformaSeguros.sln` no Visual Studio.
2. Clique com o botão direito na solução e selecione `Set Startup Projects...`.
3. Escolha `Multiple startup projects`.
4. Configure `Proposta.Api` e `Contratacao.Api` como `Start`.
5. Inicie o debug com F5.

### Rodar via VS Code

Você pode usar o comando:

```bash
dotnet run --project Proposta.Api
```

e em outro terminal:

```bash
dotnet run --project Contratacao.Api
```

Se desejar usar um arquivo de lançamento, configure um `launch.json` para os dois projetos.

### Endereços locais

| Serviço | URL |
|---|---|
| Proposta API (HTTP) | http://localhost:5001/swagger |
| Contratação API (HTTP) | http://localhost:5003/swagger |
| PostgreSQL (Docker) | localhost:5432 |


### Testes unitários

Para executar os testes do projeto:

```bash
dotnet test
```

### Fluxo de Validação: ContratacaoController

O `ContratacaoController` valida o status da proposta antes de concluir a contratação através do seguinte fluxo:

1. **HTTP POST** em `/api/contratacao` com o `PropostaId`
2. **ContratacaoUseCase** consome `IPropostaServiceClient` para buscar a proposta no `PropostaService`
3. **Validação**: Se `Status == "Aprovada"`, cria um `ContratacaoEntity` e retorna `200 OK`
4. **Rejeição**: Se Status != "Aprovada" ou a proposta não existe, retorna `400 BadRequest`

```csharp
public async Task<IActionResult> Contract([FromBody] ContratacaoRequest request)
{
    var result = await _useCase.ExecuteAsync(request.PropostaId);
    return result is null 
        ? BadRequest(new { Message = "Proposta não aprovada ou não encontrada." }) 
        : Ok(result);
}
```

### Mockando `ContratacaoController` e `ContratacaoUseCase`

O `ContratacaoController` depende do `ContratacaoUseCase`, que por sua vez consome `IPropostaServiceClient`.

#### Exemplo com Moq

```csharp
var proposta = new PropostaResponse
{
    Id = Guid.NewGuid(),
    ClienteNome = "Teste",
    Valor = 1000m,
    Status = "Aprovada"
};

var propostaClientMock = new Mock<IPropostaServiceClient>();
propostaClientMock
    .Setup(x => x.GetPropostaByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(proposta);

var useCase = new ContratacaoUseCase(propostaClientMock.Object);
var result = await useCase.ExecuteAsync(proposta.Id);

Assert.NotNull(result);
```

#### Exemplo com NSubstitute

```csharp
var proposta = new PropostaResponse
{
    Id = Guid.NewGuid(),
    ClienteNome = "Teste",
    Valor = 1000m,
    Status = "Aprovada"
};

var propostaClient = Substitute.For<IPropostaServiceClient>();
propostaClient.GetPropostaByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
    .Returns(proposta);

var useCase = new ContratacaoUseCase(propostaClient);
var result = await useCase.ExecuteAsync(proposta.Id);

Assert.NotNull(result);
```

### Observações

- O `ContratacaoUseCase` somente retorna sucesso quando a `Status` da proposta é `Aprovada`.
- Se a proposta não for encontrada ou não estiver aprovada, a contratação retorna `BadRequest`.

## Troubleshooting

- `docker-compose up -d --build`: Reconstrói imagens e sobe em segundo plano (sem bloquear o prompt).
- `docker-compose down -v`: Reseta o ambiente e remove volumes do banco de dados.

## Troubleshooting

- `docker-compose up -d --build`: Reconstrói imagens e sobe em segundo plano (sem bloquear o prompt).
- `docker-compose down -v`: Reseta o ambiente e remove volumes do banco de dados.

### URLs

- Contratação: http://localhost:5003/swagger
- Proposta: http://localhost:5001/swagger

## Próximos passos

- Adicionar testes de integração para o fluxo entre `ContratacaoService` e `PropostaService`.
- Validar a criação de propostas e contratos com o banco PostgreSQL.
