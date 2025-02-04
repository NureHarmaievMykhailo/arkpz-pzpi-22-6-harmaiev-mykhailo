# Вказуємо базовий образ для запуску застосунку .NET
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Вказуємо образ для збірки застосунку .NET (SDK образ)
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копіюємо .sln та .csproj файли в контейнер
COPY ["RoadMonitoringSystem/RoadMonitoringSystem.sln", "./"]
COPY ["RoadMonitoringSystem/RoadMonitoringSystem.csproj", "./"]

# Відновлюємо залежності
RUN dotnet restore "RoadMonitoringSystem.sln"

# Копіюємо решту файлів проекту
COPY . .

# Будуємо проект
RUN dotnet build "RoadMonitoringSystem.sln" -c Release -o /app/build

# Публікуємо застосунок
RUN dotnet publish "RoadMonitoringSystem.sln" -c Release -o /app/publish

# Створюємо фінальний контейнер для запуску застосунку
FROM base AS final
WORKDIR /app

# Копіюємо публіковані файли в фінальний контейнер
COPY --from=build /app/publish .

# Вказуємо точку входу для запуску застосунку
ENTRYPOINT ["dotnet", "RoadMonitoringSystem.dll"]
