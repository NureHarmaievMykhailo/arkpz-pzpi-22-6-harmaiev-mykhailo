# Вказуємо базовий образ для запуску застосунку .NET
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Вказуємо образ для збірки застосунку .NET (SDK образ)
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копіюємо .sln та .csproj файли в контейнер
COPY ["RoadMonitoringSystem/RoadMonitoringSystem.sln", "/src/RoadMonitoringSystem/"]
COPY ["RoadMonitoringSystem/RoadMonitoringSystem.csproj", "/src/RoadMonitoringSystem/"]

# Відновлюємо залежності
RUN dotnet restore "/src/RoadMonitoringSystem/RoadMonitoringSystem.sln"

# Копіюємо решту файлів проекту
COPY . .

# Будуємо проект
RUN dotnet build "/src/RoadMonitoringSystem/RoadMonitoringSystem.sln" -c Release -o /app/build

# Публікуємо застосунок (публікація має створити RoadMonitoringSystem.exe)
RUN dotnet publish "/src/RoadMonitoringSystem/RoadMonitoringSystem.sln" -c Release -o /app/publish

# Створюємо фінальний контейнер для запуску застосунку
FROM base AS final
WORKDIR /app

# Копіюємо публіковані файли з етапу build
COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "dotnet /app/RoadMonitoringSystem.dll"]

