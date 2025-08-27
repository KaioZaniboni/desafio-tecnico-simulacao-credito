FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SimulacaoCredito/SimulacaoCredito.csproj SimulacaoCredito/
RUN dotnet restore SimulacaoCredito/SimulacaoCredito.csproj

COPY SimulacaoCredito/ SimulacaoCredito/
WORKDIR /src/SimulacaoCredito
RUN dotnet build SimulacaoCredito.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish SimulacaoCredito.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SimulacaoCredito.dll"]
