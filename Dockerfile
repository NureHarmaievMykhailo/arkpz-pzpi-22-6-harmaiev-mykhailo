# Вказуємо базовий образ для запуску застосунку .NET
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Вказуємо образ для збірки застосунку .NET
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RoadMonitoringSystem/RoadMonitoringSystem.sln", "RoadMonitoringSystem/"]
RUN dotnet restore "RoadMonitoringSystem/RoadMonitoringSystem.sln"

# Копіюємо решту файлів проекту та будуємо застосунок
COPY . .
WORKDIR "/src/RoadMonitoringSystem"
RUN dotnet build "RoadMonitoringSystem.sln" -c Release -o /app/build

# Публікуємо застосунок
RUN dotnet publish "RoadMonitoringSystem.sln" -c Release -o /app/publish

# Створюємо фінальний контейнер для запуску застосунку
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RoadMonitoringSystem.dll"]
