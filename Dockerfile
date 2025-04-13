# Imagen base para .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SimplyTrack-API/SimplyTrack-API.csproj", "./"]
RUN dotnet restore "./SimplyTrack-API.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "SimplyTrack-API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimplyTrack-API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimplyTrack-API.dll"]
