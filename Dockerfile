# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Directory.Packages.props .
COPY Directory.Build.props .
COPY *.sln .
COPY src/SailsEnergy.Api/*.csproj src/SailsEnergy.Api/
COPY src/SailsEnergy.Application/*.csproj src/SailsEnergy.Application/
COPY src/SailsEnergy.Domain/*.csproj src/SailsEnergy.Domain/
COPY src/SailsEnergy.Infrastructure/*.csproj src/SailsEnergy.Infrastructure/
COPY src/SailsEnergy.ServiceDefaults/*.csproj src/SailsEnergy.ServiceDefaults/
COPY src/SailsEnergy.AppHost/*.csproj src/SailsEnergy.AppHost/

# Restore
RUN dotnet restore src/SailsEnergy.Api/SailsEnergy.Api.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish src/SailsEnergy.Api/SailsEnergy.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SailsEnergy.Api.dll"]
