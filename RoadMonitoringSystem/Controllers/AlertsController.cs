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
    /// Контролер для керування сповіщеннями.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор контролера.
        /// </summary>
        /// <param name="context">Контекст бази даних.</param>
        public AlertsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримує список всіх сповіщень (доступно всім авторизованим).
        /// </summary>
        /// <returns>Список сповіщень.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin, Operator, User")]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlerts()
        {
            return await _context.Alerts
                .Include(a => a.RoadSection)
                .ToListAsync();
        }

        /// <summary>
        /// Отримує сповіщення за ID (доступно всім авторизованим).
        /// </summary>
        /// <param name="id">ID сповіщення.</param>
        /// <returns>Деталі сповіщення.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Operator, User")]
        public async Task<ActionResult<Alert>> GetAlert(int id)
        {
            var alert = await _context.Alerts
                .Include(a => a.RoadSection)
                .FirstOrDefaultAsync(a => a.AlertID == id);

            if (alert == null)
            {
                return NotFound();
            }

            return alert;
        }

        /// <summary>
        /// Оновлює інформацію про сповіщення (тільки для Operator, Admin).
        /// </summary>
        /// <param name="id">ID сповіщення.</param>
        /// <param name="alert">Об'єкт із новими даними.</param>
        /// <returns>Статус оновлення.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = " Admin, Operator")]
        public async Task<IActionResult> UpdateAlert(int id, Alert alert)
        {
            if (id != alert.AlertID)
            {
                return BadRequest();
            }

            _context.Entry(alert).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlertExists(id))
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
        /// Додає нове сповіщення (тільки для Admin).
        /// </summary>
        /// <param name="alert">Об'єкт нового сповіщення.</param>
        /// <returns>Додане сповіщення.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Alert>> CreateAlert(Alert alert)
        {
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlert", new { id = alert.AlertID }, alert);
        }

        /// <summary>
        /// Видаляє сповіщення за ID (тільки для Admin).
        /// </summary>
        /// <param name="id">ID сповіщення.</param>
        /// <returns>Статус видалення.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAlert(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Перевіряє, чи існує сповіщення за ID.
        /// </summary>
        /// <param name="id">ID сповіщення.</param>
        /// <returns>True, якщо сповіщення існує.</returns>
        private bool AlertExists(int id)
        {
            return _context.Alerts.Any(a => a.AlertID == id);
        }
    }
}
