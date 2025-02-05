using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace IoTSensorSimulator.Configurations
{
    /// <summary>
    /// Менеджер конфігурації для IoT-симулятора.
    /// </summary>
    public class ConfigurationManagerO
    {
        private const string ConfigFileName = "config.json";
        public SimulatorConfig Config { get; private set; }

        /// <summary>
        /// Завантажує конфігурацію з файлу або використовує стандартні значення.
        /// </summary>
        public void Load()
        {
            if (File.Exists(ConfigFileName))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFileName);
                    Config = JsonSerializer.Deserialize<SimulatorConfig>(json) ?? GetDefaultConfig();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка читання конфігурації: {ex.Message}");
                    Config = GetDefaultConfig();
                }
            }
            else
            {
                Config = GetDefaultConfig();
                Save();
            }
        }

        /// <summary>
        /// Зберігає конфігурацію в файл.
        /// </summary>
        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження конфігурації: {ex.Message}");
            }
        }

        /// <summary>
        /// Отримує стандартну конфігурацію.
        /// </summary>
        private SimulatorConfig GetDefaultConfig()
        {
            return new SimulatorConfig
            {
                ApiEndpoint = "http://localhost:5255/api",
                DataSendInterval = 5000
            };
        }
    }

    /// <summary>
    /// Клас для збереження параметрів конфігурації.
    /// </summary>
    public class SimulatorConfig
    {
        public string ApiEndpoint { get; set; }
        public int DataSendInterval { get; set; }
    }
}
