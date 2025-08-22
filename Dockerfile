# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY SimulacaoCredito/SimulacaoCredito.csproj SimulacaoCredito/
RUN dotnet restore SimulacaoCredito/SimulacaoCredito.csproj

# Copy source code and build
COPY SimulacaoCredito/ SimulacaoCredito/
WORKDIR /src/SimulacaoCredito
RUN dotnet build SimulacaoCredito.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish SimulacaoCredito.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install dotnet-ef tool for migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "SimulacaoCredito.dll"]
