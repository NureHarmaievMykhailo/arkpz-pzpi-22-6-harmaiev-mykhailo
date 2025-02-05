using System;
using System.Collections.Generic;
using IoTSensorSimulator.Models;

namespace IoTSensorSimulator.Services
{
    /// <summary>
    /// Клас для генерації випадкових даних сенсора.
    /// Імітує показники роботи IoT-датчиків, зокрема температуру, вологість,
    /// рівень ямковості та наявність льоду.
    /// </summary>
    public class SensorDataGenerator
    {
        private readonly Random _random;

        /// <summary>
        /// Конструктор класу SensorDataGenerator.
        /// Ініціалізує генератор випадкових чисел.
        /// </summary>
        public SensorDataGenerator()
        {
            _random = new Random(); // Можна передати seed, якщо потрібно повторюваність результатів
        }

        /// <summary>
        /// Генерує випадкове значення температури в діапазоні від -10°C до +10°C.
        /// </summary>
        /// <returns>Значення температури (float).</returns>
        public float GenerateTemperature() => (float)(_random.NextDouble() * 20 - 10);

        /// <summary>
        /// Генерує випадкове значення вологості в діапазоні від 40% до 100%.
        /// </summary>
        /// <returns>Значення вологості (float).</returns>
        public float GenerateHumidity() => (float)(_random.NextDouble() * 60 + 40);

        /// <summary>
        /// Генерує випадковий показник ямковості в діапазоні від 0 до 10.
        /// </summary>
        /// <returns>Показник ямковості (float).</returns>
        public float GeneratePotholeLevel() => (float)(_random.NextDouble() * 10);

        /// <summary>
        /// Генерує показник наявності льоду: 0 – лід відсутній, 1 – лід виявлено.
        /// </summary>
        /// <returns>Ціле число (0 або 1).</returns>
        public int GenerateIcePresence() => _random.Next(0, 2);

        /// <summary>
        /// Генерує дані сенсора для декількох параметрів.
        /// Якщо необхідно, SensorID можна отримувати з конфігурації.
        /// </summary>
        /// <returns>Колекція об’єктів SensorDataDto з згенерованими даними.</returns>
        public IEnumerable<SensorDataDto> GenerateData()
        {
            // Значення SensorID можна отримати з конфігурації; тут використовується постійне значення 1
            int sensorId = 1;
            DateTime currentTimestamp = DateTime.UtcNow;

            var sensorDataList = new List<SensorDataDto>
            {
                new SensorDataDto
                {
                    SensorID = sensorId,
                    Parameter = "Температура",
                    DataValue = GenerateTemperature(),
                    Timestamp = currentTimestamp
                },
                new SensorDataDto
                {
                    SensorID = sensorId,
                    Parameter = "Вологість",
                    DataValue = GenerateHumidity(),
                    Timestamp = currentTimestamp
                },
                new SensorDataDto
                {
                    SensorID = sensorId,
                    Parameter = "Ямковість",
                    DataValue = GeneratePotholeLevel(),
                    Timestamp = currentTimestamp
                },
                new SensorDataDto
                {
                    SensorID = sensorId,
                    Parameter = "Лід",
                    DataValue = GenerateIcePresence(),
                    Timestamp = currentTimestamp
                }
            };

            return sensorDataList;
        }
    }
}
