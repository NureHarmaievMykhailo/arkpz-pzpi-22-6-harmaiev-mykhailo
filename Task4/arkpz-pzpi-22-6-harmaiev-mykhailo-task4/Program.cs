using IoTSensorSimulator.Configurations;
using IoTSensorSimulator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace IoTSensorSimulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var simulator = host.Services.GetRequiredService<IoTSimulator>();

            simulator.StartSimulation();

            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();

            await simulator.StopSimulationAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Зареєструємо конфігурацію
                    services.AddSingleton<ConfigurationManager>();

                    // Читаємо налаштування з конфігурації
                    string apiEndpoint = context.Configuration["ApiBaseAddress"] ?? "http://localhost:5255/";

                    // Зареєструємо ApiClient за допомогою фабричного методу
                    services.AddSingleton<ApiClient>(sp => new ApiClient(apiEndpoint));

                    services.AddSingleton<SensorDataGenerator, SensorDataGenerator>();
                    services.AddSingleton<IoTSimulator, IoTSimulator>();
                });
    }
}
