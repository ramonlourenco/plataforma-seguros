FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG DOTNET_RESTORE_NO_CACHE=false

COPY ["Contratacao.Api/Contratacao.Api.csproj", "Contratacao.Api/"]
COPY ["Contratacao.Application/Contratacao.Application.csproj", "Contratacao.Application/"]
COPY ["Contratacao.Domain/Contratacao.Domain.csproj", "Contratacao.Domain/"]
COPY ["Contratacao.Infrastructure/Contratacao.Infrastructure.csproj", "Contratacao.Infrastructure/"]
COPY ["Proposta.Api/Proposta.Api.csproj", "Proposta.Api/"]
COPY ["Proposta.Application/Proposta.Application.csproj", "Proposta.Application/"]
COPY ["Proposta.Domain/Proposta.Domain.csproj", "Proposta.Domain/"]
COPY ["Proposta.Infrastructure/Proposta.Infrastructure.csproj", "Proposta.Infrastructure/"]

RUN dotnet restore ${DOTNET_RESTORE_NO_CACHE:+--no-cache} "Contratacao.Api/Contratacao.Api.csproj"
COPY . .
WORKDIR /src/Contratacao.Api
RUN dotnet build -c Release "Contratacao.Api.csproj"
RUN dotnet publish -c Release -o /app --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Contratacao.Api.dll"]
