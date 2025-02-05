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
    /// Контролер для керування сповіщеннями.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _alertService;

        /// <summary>
        /// Конструктор контролера.
        /// </summary>
        /// <param name="alertService">Сервіс для роботи зі сповіщеннями.</param>
        public AlertsController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        /// <summary>
        /// Отримує список всіх сповіщень (доступно для Admin, Operator, User).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin, Operator, User")]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlerts()
        {
            var alerts = await _alertService.GetAllAlertsAsync();
            return Ok(alerts);
        }

        /// <summary>
        /// Отримує сповіщення за ID (доступно для Admin, Operator, User).
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Operator, User")]
        public async Task<ActionResult<Alert>> GetAlert(int id)
        {
            var alert = await _alertService.GetAlertByIdAsync(id);
            if (alert == null)
            {
                return NotFound();
            }
            return Ok(alert);
        }

        /// <summary>
        /// Оновлює інформацію про сповіщення (тільки для Admin, Operator).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> UpdateAlert(int id, AlertDto alert)
        {
            var success = await _alertService.UpdateAlertAsync(id, alert);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Додає нове сповіщення (тільки для Admin). Використовує DTO для спрощення введення.\n
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Alert>> CreateAlert([FromBody] AlertDto alertDto)
        {
            var newAlert = await _alertService.CreateAlertAsync(alertDto);
            return CreatedAtAction(nameof(GetAlert), new { id = newAlert.AlertID }, newAlert);
        }

        /// <summary>
        /// Видаляє сповіщення за ID (тільки для Admin).
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAlert(int id)
        {
            var success = await _alertService.DeleteAlertAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Позначає сповіщення як вирішене (тільки для Admin, Operator).\n
        /// </summary>
        [HttpPut("{id}/resolve")]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> MarkAlertResolved(int id)
        {
            var success = await _alertService.MarkAlertResolvedAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Генерує критичні сповіщення на основі даних сенсорів (тільки для Admin).\n
        /// </summary>
        [HttpPost("generate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateCriticalAlerts()
        {
            await _alertService.GenerateCriticalAlertsAsync();
            return Ok(new { message = "Critical alerts generated." });
        }
    }
}
