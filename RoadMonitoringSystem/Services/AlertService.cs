using Microsoft.EntityFrameworkCore;
using RoadMonitoringSystem.Data;
using RoadMonitoringSystem.DTO;
using RoadMonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadMonitoringSystem.Services
{
    /// <summary>
    /// Інтерфейс для сервісу управління сповіщеннями.
    /// </summary>
    public interface IAlertService
    {
        Task<IEnumerable<Alert>> GetAllAlertsAsync();
        Task<Alert?> GetAlertByIdAsync(int id);
        Task<Alert> CreateAlertAsync(AlertDto alertDto);
        Task<bool> UpdateAlertAsync(int id, Alert alert);
        Task<bool> DeleteAlertAsync(int id);
        Task<bool> MarkAlertResolvedAsync(int id);
        // Метод для генерації критичних сповіщень (імітація роботи IoT сенсорів)
        Task GenerateCriticalAlertsAsync();
    }

    /// <summary>
    /// Сервіс для управління сповіщеннями.
    /// </summary>
    public class AlertService : IAlertService
    {
        private readonly ApplicationDbContext _context;

        public AlertService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримує список всіх сповіщень.
        /// </summary>
        public async Task<IEnumerable<Alert>> GetAllAlertsAsync()
        {
            return await _context.Alerts.Include(a => a.RoadSection).ToListAsync();
        }

        /// <summary>
        /// Отримує сповіщення за ID.
        /// </summary>
        public async Task<Alert?> GetAlertByIdAsync(int id)
        {
            return await _context.Alerts
                .Include(a => a.RoadSection)
                .FirstOrDefaultAsync(a => a.AlertID == id);
        }

        /// <summary>
        /// Створює нове сповіщення на основі даних з DTO.
        /// </summary>
        public async Task<Alert> CreateAlertAsync(AlertDto alertDto)
        {
            var alert = new Alert
            {
                RoadSectionID = alertDto.RoadSectionID,
                AlertType = alertDto.AlertType,
                Message = alertDto.Message,
                CreatedDate = DateTime.UtcNow,
                IsResolved = false
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        /// <summary>
        /// Оновлює дані сповіщення (наприклад, для відмітки про вирішення).
        /// </summary>
        public async Task<bool> UpdateAlertAsync(int id, Alert alert)
        {
            if (id != alert.AlertID)
            {
                return false;
            }

            _context.Entry(alert).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Alerts.AnyAsync(a => a.AlertID == id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Видаляє сповіщення за ID.
        /// </summary>
        public async Task<bool> DeleteAlertAsync(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return false;
            }

            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Позначає сповіщення як вирішене (IsResolved = true).
        /// </summary>
        public async Task<bool> MarkAlertResolvedAsync(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return false;
            }
            alert.IsResolved = true;
            _context.Entry(alert).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Генерує критичні сповіщення на основі даних сенсорів (імітація роботи IoT-датчиків).\n
        /// Приклад логіки: якщо температура нижче певного порогу або рівень вологості перевищує критичний рівень,\n
        /// створюється сповіщення для відповідної ділянки дороги.\n
        /// (Реалізація може бути розширена відповідно до вимог.)\n
        /// </summary>
        public async Task GenerateCriticalAlertsAsync()
        {
            // Отримуємо дані з SensorData, які відповідають критичним показникам
            // Наприклад, якщо показник "Температура" < -5 або "Вологість" > 80.
            var criticalTemperatureData = await _context.SensorData
                .Include(sd => sd.Sensor)
                .ThenInclude(s => s.RoadSection)
                .Where(sd => sd.Parameter == "Температура" && sd.DataValue < -5)
                .ToListAsync();

            foreach (var data in criticalTemperatureData)
            {
                // Перевіряємо, чи вже існує подібне сповіщення для даної ділянки
                bool exists = await _context.Alerts.AnyAsync(a =>
                    a.RoadSectionID == data.Sensor.RoadSectionID &&
                    a.AlertType == "Температура" &&
                    !a.IsResolved);

                if (!exists)
                {
                    var alert = new Alert
                    {
                        RoadSectionID = data.Sensor.RoadSectionID,
                        AlertType = "Температура",
                        Message = $"Температура {data.DataValue}°C. Ризик обледеніння.",
                        CreatedDate = DateTime.UtcNow,
                        IsResolved = false
                    };

                    _context.Alerts.Add(alert);
                }
            }

            // Аналогічну логіку можна додати для інших параметрів (Вологість, Ямковість, Лід тощо)
            await _context.SaveChangesAsync();
        }
    }
}
