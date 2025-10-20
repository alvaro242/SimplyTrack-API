# Use official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (for better layer caching)
COPY ["SimplyTrack-API.csproj", "./"]
RUN dotnet restore "SimplyTrack-API.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "SimplyTrack-API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "SimplyTrack-API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "SimplyTrackAPI.dll"]