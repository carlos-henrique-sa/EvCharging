# Usando .NET 9 SDK (ou mude para 8.0 se preferir LTS)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["EvCharging.csproj", "./"]
RUN dotnet restore "EvCharging.csproj"

COPY . .
WORKDIR "/src"
RUN dotnet publish "EvCharging.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EvCharging.dll"]