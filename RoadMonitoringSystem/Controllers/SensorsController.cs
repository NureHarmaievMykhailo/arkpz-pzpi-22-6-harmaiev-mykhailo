using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RoadMonitoringSystem.Models;
using RoadMonitoringSystem.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoadMonitoringSystem.DTO;

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
        private readonly ISensorService _sensorService;

        /// <summary>
        /// Конструктор контролера.
        /// </summary>
        /// <param name="sensorService">Сервіс для роботи з сенсорами.</param>
        public SensorsController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        /// <summary>
        /// Отримує список всіх сенсорів (доступно для User, Operator, Admin).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<IEnumerable<Sensor>>> GetSensors()
        {
            var sensors = await _sensorService.GetAllSensorsAsync();
            return Ok(sensors);
        }

        /// <summary>
        /// Отримує деталі сенсора за ID (доступно для User, Operator, Admin).
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<Sensor>> GetSensor(int id)
        {
            var sensor = await _sensorService.GetSensorByIdAsync(id);
            if (sensor == null)
            {
                return NotFound();
            }
            return Ok(sensor);
        }

        /// <summary>
        /// Оновлює інформацію про сенсор (тільки для Operator, Admin).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Operator, Admin")]
        public async Task<IActionResult> UpdateSensor(int id, Sensor sensor)
        {
            var success = await _sensorService.UpdateSensorAsync(id, sensor);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Додає новий сенсор (тільки для Admin).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Sensor>> CreateSensor(SensorDto sensorDto)
        {
            var newSensor = await _sensorService.CreateSensorAsync(sensorDto);
            return CreatedAtAction(nameof(GetSensor), new { id = newSensor.SensorID }, newSensor);
        }

        /// <summary>
        /// Видаляє сенсор за ID (тільки для Admin).
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSensor(int id)
        {
            var success = await _sensorService.DeleteSensorAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
