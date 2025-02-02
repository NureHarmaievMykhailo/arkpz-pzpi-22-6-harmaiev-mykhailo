using Microsoft.EntityFrameworkCore;
using RoadMonitoringSystem.Data;
using RoadMonitoringSystem.DTO;
using RoadMonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace RoadMonitoringSystem.Services
{
    public interface ISensorDataService
    {
        Task<IEnumerable<SensorData>> GetAllSensorDataAsync();
        Task<SensorData?> GetSensorDataByIdAsync(int id);
        Task<SensorData> CreateSensorDataAsync(SensorData sensorData);
        Task<SensorData> CreateSensorDataAsync(SensorDataDto dto);
        Task<bool> UpdateSensorDataAsync(int id, SensorData sensorData);
        Task<bool> DeleteSensorDataAsync(int id);
        Task<string> GenerateAnalyticalReportAsync();
    }

    public class SensorDataService : ISensorDataService
    {
        private readonly ApplicationDbContext _context;

        private readonly string[] allowedParameters = { "Температура", "Вологість", "Ямковість", "Лід" };

        public SensorDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SensorData>> GetAllSensorDataAsync()
        {
            return await _context.SensorData
                .Include(sd => sd.Sensor)
                    .ThenInclude(s => s.RoadSection)
                .ToListAsync();
        }

        public async Task<SensorData?> GetSensorDataByIdAsync(int id)
        {
            return await _context.SensorData
                .Include(sd => sd.Sensor)
                    .ThenInclude(s => s.RoadSection)
                .FirstOrDefaultAsync(sd => sd.SensorDataID == id);
        }

        public async Task<SensorData> CreateSensorDataAsync(SensorData sensorData)
        {
            _context.SensorData.Add(sensorData);
            await _context.SaveChangesAsync();

            var sensor = await _context.Sensors.FindAsync(sensorData.SensorID);
            if (sensor != null)
            {
                await CheckAndSaveCriticalAlerts(sensor.RoadSectionID, sensorData.Parameter, sensorData.DataValue);
            }

            await AnalyzeSensorDataAsync(sensorData);

            return sensorData;
        }

        public async Task<SensorData> CreateSensorDataAsync(SensorDataDto dto)
        {
            // Перевірка, чи належить Parameter до дозволених
            if (!allowedParameters.Contains(dto.Parameter))
            {
                throw new ArgumentException($"Неприпустимий параметр '{dto.Parameter}'. Дозволені значення: {string.Join(", ", allowedParameters)}");
            }

            var sensor = await _context.Sensors.FindAsync(dto.SensorID);
            if (sensor == null)
            {
                throw new ArgumentException($"Сенсор з ID {dto.SensorID} не знайдено.");
            }

            var sensorData = new SensorData
            {
                SensorID = dto.SensorID,
                Parameter = dto.Parameter,
                DataValue = dto.DataValue,
                Timestamp = dto.Timestamp
            };

            return await CreateSensorDataAsync(sensorData);
        }

        public async Task<bool> UpdateSensorDataAsync(int id, SensorData sensorData)
        {
            if (id != sensorData.SensorDataID)
            {
                return false;
            }

            _context.Entry(sensorData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                var sensor = await _context.Sensors.FindAsync(sensorData.SensorID);
                if (sensor != null)
                {
                    await CheckAndSaveCriticalAlerts(sensor.RoadSectionID, sensorData.Parameter, sensorData.DataValue);
                }
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SensorDataExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteSensorDataAsync(int id)
        {
            var sensorData = await _context.SensorData.FindAsync(id);
            if (sensorData == null)
            {
                return false;
            }

            _context.SensorData.Remove(sensorData);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateAnalyticalReportAsync()
        {
            var sensorDataList = await _context.SensorData.ToListAsync();
            int totalEntries = sensorDataList.Count;
            double averageValue = sensorDataList.Any() ? sensorDataList.Average(sd => sd.DataValue) : 0;

            string report = $"Аналітичний звіт: Загальна кількість записів = {totalEntries}, середнє значення = {averageValue:F2}";
            return report;
        }

        private async Task<bool> SensorDataExistsAsync(int id)
        {
            return await _context.SensorData.AnyAsync(sd => sd.SensorDataID == id);
        }

        /// <summary>
        /// Аналізує отримані дані сенсора та генерує сповіщення при досягненні критичних значень.
        /// Логіка:
        ///  - Температура < -5°C → ризик обледеніння.
        ///  - Вологість > 80% і Температура < 3°C → ризик ожеледиці.
        ///  - Ямковість > 3 см → виявлено нерівність дороги.
        ///  - Лід = 1.0 → необхідність посипання.
        /// </summary>
        private async Task AnalyzeSensorDataAsync(SensorData sensorData)
        {
            switch (sensorData.Parameter)
            {
                case "Температура":
                    if (sensorData.DataValue < -5)
                    {
                        Notify($"[ALERT] Ризик обледеніння! Температура: {sensorData.DataValue}°C (SensorID: {sensorData.SensorID}).");
                    }
                    break;

                case "Вологість":
                    if (sensorData.DataValue > 80)
                    {
                        var temperatureData = await _context.SensorData
                            .Include(sd => sd.Sensor)
                            .Where(sd => sd.Sensor.RoadSectionID == sensorData.Sensor.RoadSectionID &&
                                         sd.Sensor.SensorType == "Температура")
                            .OrderByDescending(sd => sd.Timestamp)
                            .FirstOrDefaultAsync();

                        if (temperatureData != null && temperatureData.DataValue < 3)
                        {
                            Notify($"[ALERT] Ризик ожеледиці! Вологість: {sensorData.DataValue}% і температура: {temperatureData.DataValue}°C (RoadSectionID: {sensorData.Sensor.RoadSectionID}).");
                        }
                    }
                    break;

                case "Ямковість":
                    if (sensorData.DataValue > 3)
                    {
                        Notify($"[ALERT] Виявлено нерівність дороги! Ямковість: {sensorData.DataValue} см (SensorID: {sensorData.SensorID}).");
                    }
                    break;

                case "Лід":
                    if (sensorData.DataValue == 1.0)
                    {
                        Notify($"[ALERT] Наявність льоду! Необхідно посипати (SensorID: {sensorData.SensorID}).");
                    }
                    break;

                default:
                    break;
            }

            await Task.CompletedTask;
        }

        private void Notify(string message)
        {
            try
            {
                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;

                    // Замініть 'your_email@gmail.com' на вашу адресу, а 'your_app_password' - на пароль додатку, який ви згенерували
                    smtpClient.Credentials = new NetworkCredential("mykhailo.harmaiev@nure.ua", "dwuz zcdc zdkp yuch");

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("mykhailo.harmaiev@nure.ua"),
                        Subject = "Критичне сповіщення",
                        Body = message,
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add("sayanheck@gmail.com");

                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Помилка при відправці email: {ex.Message}");
            }
        }

        private async Task CheckAndSaveCriticalAlerts(int roadSectionId, string parameter, double dataValue)
        {
            Alert alert = null;

            if (parameter == "Температура" && dataValue < -5)
            {
                alert = new Alert
                {
                    RoadSectionID = roadSectionId,
                    AlertType = "Температура",
                    Message = "Температура нижче -5°C. Можливе обледеніння.",
                    CreatedDate = DateTime.UtcNow, // Явно встановлюємо дату створення
                    IsResolved = false
                };
            }
            else if (parameter == "Вологість" && dataValue > 80)
            {
                alert = new Alert
                {
                    RoadSectionID = roadSectionId,
                    AlertType = "Вологість",
                    Message = "Рівень вологості перевищує 80%. Ризик ожеледиці.",
                    CreatedDate = DateTime.UtcNow,
                    IsResolved = false
                };
            }
            else if (parameter == "Ямковість" && dataValue > 3)
            {
                alert = new Alert
                {
                    RoadSectionID = roadSectionId,
                    AlertType = "Ямковість",
                    Message = "Виявлено нерівність дороги (ямковість більше 3 см).",
                    CreatedDate = DateTime.UtcNow,
                    IsResolved = false
                };
            }
            else if (parameter == "Лід" && dataValue == 1.0)
            {
                alert = new Alert
                {
                    RoadSectionID = roadSectionId,
                    AlertType = "Лід",
                    Message = "Виявлено лід на дорозі. Необхідне посипання.",
                    CreatedDate = DateTime.UtcNow,
                    IsResolved = false
                };
            }

            if (alert != null)
            {
                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();
            }
        }

    }
}

