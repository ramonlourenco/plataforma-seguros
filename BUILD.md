# Como instalar e executar o projeto

## Pré-requisitos

1. **Instalar .NET 8 SDK**
   - Baixe e instale o .NET 8 SDK de: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verifique a instalação: `dotnet --version`

2. **Verificar instalação**
   ```bash
   dotnet --version
   # Deve mostrar algo como: 8.0.xxx
   ```

## Como executar

1. **Navegar para o diretório do projeto**
   ```bash
   cd C:\Users\Thais Gualberto\Documents\Features\HexagonalMicroservices
   ```

2. **Restaurar dependências**
   ```bash
   dotnet restore
   ```

3. **Compilar a solução**
   ```bash
   dotnet build
   ```

4. **Executar os testes**
   ```bash
   dotnet test
   ```

5. **Executar os serviços**
   ```bash
   # Terminal 1 - PropostaService
   cd PropostaService.Api
   dotnet run --urls=http://localhost:5001

   # Terminal 2 - ContratacaoService
   cd ContratacaoService.Api
   dotnet run --urls=http://localhost:5002
   ```

## Com Docker (opcional)

```bash
docker-compose up --build
```

## URLs dos serviços

- PropostaService: http://localhost:5001/swagger
- ContratacaoService: http://localhost:5002/swagger