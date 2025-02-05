using Microsoft.EntityFrameworkCore;
using RoadMonitoringSystem.Data;
using RoadMonitoringSystem.DTO;
using RoadMonitoringSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadMonitoringSystem.Services
{
    
    public interface ISensorService
    {
        Task<IEnumerable<Sensor>> GetAllSensorsAsync();
        Task<Sensor?> GetSensorByIdAsync(int id);
        Task<Sensor> CreateSensorAsync(SensorDto sensorDto);
        Task<bool> UpdateSensorAsync(int id, SensorDto sensorDto);
        Task<bool> DeleteSensorAsync(int id);
    }

    public class SensorService : ISensorService
    {
        private readonly ApplicationDbContext _context;

        public SensorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sensor>> GetAllSensorsAsync()
        {
            return await _context.Sensors
                .Include(s => s.RoadSection)
                .ToListAsync();
        }

        public async Task<Sensor?> GetSensorByIdAsync(int id)
        {
            return await _context.Sensors
                .Include(s => s.RoadSection)
                .FirstOrDefaultAsync(s => s.SensorID == id);
        }

        public async Task<Sensor> CreateSensorAsync(SensorDto sensorDto)
        {

            var validSensorTypes = new List<string> { "Температура", "Вологість", "Лід", "Ямковість" };

            var validStatuses = new List<string> { "Активний", "Не працює" };

            var roadSection = await _context.RoadSections.FindAsync(sensorDto.RoadSectionID);
            if (roadSection == null)
            {
                throw new KeyNotFoundException("Вказана ділянка дороги не існує.");
            }

            if (!validSensorTypes.Contains(sensorDto.SensorType))
            {
                throw new ArgumentException($"Некоректний тип сенсора: {sensorDto.SensorType}. Доступні: {string.Join(", ", validSensorTypes)}.");
            }

            if (!validStatuses.Contains(sensorDto.Status))
            {
                throw new ArgumentException($"Некоректний статус сенсора: {sensorDto.Status}. Доступні: {string.Join(", ", validStatuses)}.");
            }

            var newSensor = new Sensor
            {
                RoadSectionID = sensorDto.RoadSectionID,
                SensorType = sensorDto.SensorType,
                InstallationDate = sensorDto.InstallationDate,
                Status = sensorDto.Status
            };

            _context.Sensors.Add(newSensor);
            await _context.SaveChangesAsync();
            return newSensor;
        }


        public async Task<bool> UpdateSensorAsync(int id, SensorDto sensorDto)
        {
            var validSensorTypes = new List<string> { "Температура", "Вологість", "Лід", "Ямковість" };
            var validStatuses = new List<string> { "Активний", "Не працює" };

            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null)
            {
                return false;
            }

            var roadSection = await _context.RoadSections.FindAsync(sensorDto.RoadSectionID);
            if (roadSection == null)
            {
                throw new KeyNotFoundException("Вказана ділянка дороги не існує.");
            }

            if (!validSensorTypes.Contains(sensorDto.SensorType))
            {
                throw new ArgumentException($"Некоректний тип сенсора: {sensorDto.SensorType}. Доступні: {string.Join(", ", validSensorTypes)}.");
            }

            if (!validStatuses.Contains(sensorDto.Status))
            {
                throw new ArgumentException($"Некоректний статус сенсора: {sensorDto.Status}. Доступні: {string.Join(", ", validStatuses)}.");
            }

            // Оновлення полів сенсора
            sensor.RoadSectionID = sensorDto.RoadSectionID;
            sensor.SensorType = sensorDto.SensorType;
            sensor.InstallationDate = sensorDto.InstallationDate;
            sensor.Status = sensorDto.Status;

            _context.Entry(sensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Sensors.AnyAsync(s => s.SensorID == id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }


        public async Task<bool> DeleteSensorAsync(int id)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null)
            {
                return false;
            }

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
