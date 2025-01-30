using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RoadMonitoringSystem.Data;
using RoadMonitoringSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadMonitoringSystem.Controllers
{
    /// <summary>
    /// Контролер для керування даними сенсорів.
    /// </summary>
    [Authorize] // Всі методи вимагають авторизації
    [Route("api/[controller]")]
    [ApiController]
    public class SensorDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор контролера.
        /// </summary>
        /// <param name="context">Контекст бази даних.</param>
        public SensorDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримує список всіх даних сенсорів.
        /// </summary>
        /// <returns>Список даних сенсорів.</returns>
        [HttpGet]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<IEnumerable<SensorData>>> GetSensorData()
        {
            return await _context.SensorData
                .Include(sd => sd.Sensor)
                .ThenInclude(s => s.RoadSection)
                .ToListAsync();
        }

        /// <summary>
        /// Отримує дані сенсора за ID.
        /// </summary>
        /// <param name="id">ID даних сенсора.</param>
        /// <returns>Деталі даних сенсора.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<SensorData>> GetSensorDataById(int id)
        {
            var sensorData = await _context.SensorData
                .Include(sd => sd.Sensor)
                .ThenInclude(s => s.RoadSection)
                .FirstOrDefaultAsync(sd => sd.SensorDataID == id);

            if (sensorData == null)
            {
                return NotFound();
            }

            return sensorData;
        }

        /// <summary>
        /// Оновлює інформацію про дані сенсора.
        /// </summary>
        /// <param name="id">ID даних сенсора.</param>
        /// <param name="sensorData">Об'єкт із новими даними.</param>
        /// <returns>Статус оновлення.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Operator, Admin")]
        public async Task<IActionResult> UpdateSensorData(int id, SensorData sensorData)
        {
            if (id != sensorData.SensorDataID)
            {
                return BadRequest();
            }

            _context.Entry(sensorData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SensorDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Додає нові дані сенсора.
        /// </summary>
        /// <param name="sensorData">Об'єкт нових даних сенсора.</param>
        /// <returns>Додані дані сенсора.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SensorData>> CreateSensorData(SensorData sensorData)
        {
            _context.SensorData.Add(sensorData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSensorDataById", new { id = sensorData.SensorDataID }, sensorData);
        }

        /// <summary>
        /// Видаляє дані сенсора за ID.
        /// </summary>
        /// <param name="id">ID даних сенсора.</param>
        /// <returns>Статус видалення.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSensorData(int id)
        {
            var sensorData = await _context.SensorData.FindAsync(id);
            if (sensorData == null)
            {
                return NotFound();
            }

            _context.SensorData.Remove(sensorData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Перевіряє, чи існують дані сенсора за ID.
        /// </summary>
        /// <param name="id">ID даних сенсора.</param>
        /// <returns>True, якщо дані існують.</returns>
        private bool SensorDataExists(int id)
        {
            return _context.SensorData.Any(sd => sd.SensorDataID == id);
        }
    }
}