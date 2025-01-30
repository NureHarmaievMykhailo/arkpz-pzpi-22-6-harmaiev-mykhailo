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
    /// Контролер для керування сенсорами.
    /// </summary>
    [Authorize] // Всі методи вимагають авторизації
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор контролера.
        /// </summary>
        /// <param name="context">Контекст бази даних.</param>
        public SensorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримує список всіх сенсорів (доступно всім авторизованим).
        /// </summary>
        /// <returns>Список сенсорів.</returns>
        [HttpGet]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<IEnumerable<Sensor>>> GetSensors()
        {
            return await _context.Sensors
                .Include(s => s.RoadSection)
                .ToListAsync();
        }

        /// <summary>
        /// Отримує сенсор за ID (доступно всім авторизованим).
        /// </summary>
        /// <param name="id">ID сенсора.</param>
        /// <returns>Деталі сенсора.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<Sensor>> GetSensor(int id)
        {
            var sensor = await _context.Sensors
                .Include(s => s.RoadSection)
                .FirstOrDefaultAsync(s => s.SensorID == id);

            if (sensor == null)
            {
                return NotFound();
            }

            return sensor;
        }

        /// <summary>
        /// Оновлює інформацію про сенсор (тільки для Operator, Admin).
        /// </summary>
        /// <param name="id">ID сенсора.</param>
        /// <param name="sensor">Об'єкт із новими даними.</param>
        /// <returns>Статус оновлення.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Operator, Admin")]
        public async Task<IActionResult> UpdateSensor(int id, Sensor sensor)
        {
            if (id != sensor.SensorID)
            {
                return BadRequest();
            }

            _context.Entry(sensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SensorExists(id))
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
        /// Додає новий сенсор (тільки для Admin).
        /// </summary>
        /// <param name="sensor">Об'єкт нового сенсора.</param>
        /// <returns>Доданий сенсор.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Sensor>> CreateSensor(Sensor sensor)
        {
            _context.Sensors.Add(sensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSensor", new { id = sensor.SensorID }, sensor);
        }

        /// <summary>
        /// Видаляє сенсор за ID (тільки для Admin).
        /// </summary>
        /// <param name="id">ID сенсора.</param>
        /// <returns>Статус видалення.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSensor(int id)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null)
            {
                return NotFound();
            }

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Перевіряє, чи існує сенсор за ID.
        /// </summary>
        /// <param name="id">ID сенсора.</param>
        /// <returns>True, якщо сенсор існує.</returns>
        private bool SensorExists(int id)
        {
            return _context.Sensors.Any(s => s.SensorID == id);
        }
    }
}