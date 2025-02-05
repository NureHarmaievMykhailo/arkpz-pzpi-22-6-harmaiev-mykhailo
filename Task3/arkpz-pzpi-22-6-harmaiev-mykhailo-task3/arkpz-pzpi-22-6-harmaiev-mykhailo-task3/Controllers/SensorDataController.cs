using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RoadMonitoringSystem.DTO;
using RoadMonitoringSystem.Models;
using RoadMonitoringSystem.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadMonitoringSystem.Controllers
{
    /// <summary>
    /// Контролер для керування даними сенсорів.
    /// </summary>
    ///[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SensorDataController : ControllerBase
    {
        private readonly ISensorDataService _sensorDataService;

        /// <summary>
        /// Конструктор контролера SensorDataController.
        /// </summary>
        /// <param name="sensorDataService">Сервіс для обробки даних сенсорів.</param>
        public SensorDataController(ISensorDataService sensorDataService)
        {
            _sensorDataService = sensorDataService;
        }

        /// <summary>
        /// Отримує всі дані сенсорів.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<IEnumerable<SensorData>>> GetSensorData()
        {
            var sensorData = await _sensorDataService.GetAllSensorDataAsync();
            return Ok(sensorData);
        }

        /// <summary>
        /// Отримує дані сенсора за ідентифікатором.
        /// </summary>
        /// <param name="id">Ідентифікатор сенсора.</param>
        [HttpGet("{id}")]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<SensorData>> GetSensorDataById(int id)
        {
            var sensorData = await _sensorDataService.GetSensorDataByIdAsync(id);
            if (sensorData == null)
            {
                return NotFound();
            }
            return Ok(sensorData);
        }

        /// <summary>
        /// Оновлює дані сенсора.
        /// </summary>
        /// <param name="id">Ідентифікатор сенсора.</param>
        /// <param name="sensorData">Об'єкт сенсорних даних для оновлення.</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "Operator, Admin")]
        public async Task<IActionResult> UpdateSensorData(int id, SensorDataDto sensorData)
        {
            var success = await _sensorDataService.UpdateSensorDataAsync(id, sensorData);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Додає нові дані сенсора. Приймає спрощений об'єкт DTO.
        /// </summary>
        /// <param name="createDto">Об'єкт DTO з вхідними даними сенсора.</param>
        [HttpPost]
        ///[Authorize(Roles = "Admin")]
        public async Task<ActionResult<SensorData>> CreateSensorData([FromBody] SensorDataDto createDto)
        {
            try
            {
                var newSensorData = await _sensorDataService.CreateSensorDataAsync(createDto);
                return CreatedAtAction(nameof(GetSensorDataById), new { id = newSensorData.SensorDataID }, newSensorData);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Видаляє дані сенсора за ідентифікатором.
        /// </summary>
        /// <param name="id">Ідентифікатор сенсора.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSensorData(int id)
        {
            var success = await _sensorDataService.DeleteSensorDataAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Генерує аналітичний звіт за даними сенсорів.
        /// </summary>
        [HttpGet("report")]
        [Authorize(Roles = "Operator, Admin")]
        public async Task<ActionResult<string>> GetAnalyticalReport()
        {
            var report = await _sensorDataService.GenerateAnalyticalReportAsync();
            return Ok(report);
        }
    }
}