# Etapa de build - usando SDK do .NET 10.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia apenas o csproj primeiro (melhor cache do Docker)
COPY ["EvCharging.csproj", "./"]
RUN dotnet restore "EvCharging.csproj"

# Copia o resto do código
COPY . .
WORKDIR "/src"
RUN dotnet publish "EvCharging.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final - runtime leve
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "EvCharging.dll"]