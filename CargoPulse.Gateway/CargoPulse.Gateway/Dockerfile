FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CargoPulse.Gateway.csproj", "."]
RUN dotnet restore "CargoPulse.Gateway.csproj"

COPY . .
RUN dotnet build "CargoPulse.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CargoPulse.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CargoPulse.Gateway.dll"]