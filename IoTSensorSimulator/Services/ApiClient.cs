using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IoTSensorSimulator.Models;

namespace IoTSensorSimulator.Services
{
    /// <summary>
    /// Клас для взаємодії з API серверної частини.
    /// Відправляє згенеровані дані сенсора на сервер.
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiEndpoint;

        /// <summary>
        /// Ініціалізує новий екземпляр класу ApiClient.
        /// </summary>
        /// <param name="apiEndpoint">Базова URL-адреса серверного API.</param>
        public ApiClient(string apiEndpoint)
        {
            _httpClient = new HttpClient();
            _apiEndpoint = apiEndpoint.TrimEnd('/');
        }

        /// <summary>
        /// Відправляє дані сенсора на сервер.
        /// </summary>
        /// <param name="sensorData">Об'єкт SensorDataDto з показниками сенсора.</param>
        public async Task SendSensorDataAsync(SensorDataDto sensorData)
        {
            try
            {
                string jsonData = JsonSerializer.Serialize(sensorData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var requestUrl = $"{_apiEndpoint}/api/SensorData";
                Console.WriteLine($"Відправка даних за URL: {requestUrl}");
                HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Помилка відправки: {response.StatusCode}. Деталі: {errorContent}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Виникла помилка при запиті: {ex.Message}");
            }
        }

    }
}
