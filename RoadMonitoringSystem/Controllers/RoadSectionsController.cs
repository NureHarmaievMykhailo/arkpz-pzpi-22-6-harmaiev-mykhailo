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
    /// Контролер для керування ділянками доріг.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoadSectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор контролера.
        /// </summary>
        /// <param name="context">Контекст бази даних.</param>
        public RoadSectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримує список всіх ділянок доріг (доступно всім авторизованим).
        /// </summary>
        /// <returns>Список ділянок доріг.</returns>
        [HttpGet]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<IEnumerable<RoadSection>>> GetRoadSections()
        {
            return await _context.RoadSections.Include(r => r.Sensors).ToListAsync();
        }

        /// <summary>
        /// Отримує ділянку дороги за ID (доступно всім авторизованим).
        /// </summary>
        /// <param name="id">ID ділянки.</param>
        /// <returns>Деталі ділянки дороги.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<RoadSection>> GetRoadSection(int id)
        {
            var roadSection = await _context.RoadSections
                .Include(r => r.Sensors)
                .FirstOrDefaultAsync(r => r.RoadSectionID == id);

            if (roadSection == null)
            {
                return NotFound();
            }

            return roadSection;
        }

        /// <summary>
        /// Оновлює інформацію про ділянку дороги (тільки для Operator, Admin).
        /// </summary>
        /// <param name="id">ID ділянки.</param>
        /// <param name="roadSection">Об'єкт із новими даними.</param>
        /// <returns>Статус оновлення.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Operator, Admin")]
        public async Task<IActionResult> UpdateRoadSection(int id, RoadSection roadSection)
        {
            if (id != roadSection.RoadSectionID)
            {
                return BadRequest();
            }

            _context.Entry(roadSection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoadSectionExists(id))
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
        /// Додає нову ділянку дороги (тільки для Admin).
        /// </summary>
        /// <param name="roadSection">Об'єкт нової ділянки дороги.</param>
        /// <returns>Додана ділянка дороги.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoadSection>> CreateRoadSection(RoadSection roadSection)
        {
            _context.RoadSections.Add(roadSection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoadSection", new { id = roadSection.RoadSectionID }, roadSection);
        }

        /// <summary>
        /// Видаляє ділянку дороги за ID (тільки для Admin).
        /// </summary>
        /// <param name="id">ID ділянки.</param>
        /// <returns>Статус видалення.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoadSection(int id)
        {
            var roadSection = await _context.RoadSections.FindAsync(id);
            if (roadSection == null)
            {
                return NotFound();
            }

            _context.RoadSections.Remove(roadSection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Перевіряє, чи існує ділянка дороги за ID.
        /// </summary>
        /// <param name="id">ID ділянки.</param>
        /// <returns>True, якщо існує.</returns>
        private bool RoadSectionExists(int id)
        {
            return _context.RoadSections.Any(r => r.RoadSectionID == id);
        }
    }
}
