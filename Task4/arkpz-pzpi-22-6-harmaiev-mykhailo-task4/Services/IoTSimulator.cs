using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IoTSensorSimulator.Models;
using IoTSensorSimulator.Services;

namespace IoTSensorSimulator.Services
{
    /// <summary>
    /// Основний клас для симуляції роботи IoT-сенсора.
    /// </summary>
    public class IoTSimulator
    {
        private readonly SensorDataGenerator _sensorDataGenerator;
        private readonly ApiClient _apiClient;
        private readonly int _intervalMilliseconds;
        private bool _isRunning;
        private Task _simulationTask;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Ініціалізує новий екземпляр IoTSimulator.
        /// </summary>
        /// <param name="sensorDataGenerator">Об'єкт для генерації даних сенсорів.</param>
        /// <param name="apiClient">Клієнт для взаємодії з сервером.</param>
        /// <param name="intervalMilliseconds">Інтервал відправки даних у мілісекундах.</param>
        public IoTSimulator(SensorDataGenerator sensorDataGenerator, ApiClient apiClient, int intervalMilliseconds = 5000)
        {
            _sensorDataGenerator = sensorDataGenerator ?? throw new ArgumentNullException(nameof(sensorDataGenerator));
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _intervalMilliseconds = intervalMilliseconds;
            _isRunning = false;
        }

        /// <summary>
        /// Запускає симуляцію.
        /// </summary>
        public void StartSimulation()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _simulationTask = Task.Run(() => RunSimulationAsync(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Зупиняє симуляцію.
        /// </summary>
        public async Task StopSimulationAsync()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _cancellationTokenSource.Cancel();

            try
            {
                await _simulationTask;
            }
            catch (TaskCanceledException) { }
            finally
            {
                _cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// Основний цикл генерації та відправки даних.
        /// </summary>
        private async Task RunSimulationAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var sensorDataList = _sensorDataGenerator.GenerateData();
                foreach (var sensorData in sensorDataList)
                {
                    await _apiClient.SendSensorDataAsync(sensorData);
                    Console.WriteLine($"Відправлено: {sensorData.Timestamp} | {sensorData.Parameter}: {sensorData.DataValue}");
                }
                await Task.Delay(_intervalMilliseconds, cancellationToken);
            }
        }
    }
}
