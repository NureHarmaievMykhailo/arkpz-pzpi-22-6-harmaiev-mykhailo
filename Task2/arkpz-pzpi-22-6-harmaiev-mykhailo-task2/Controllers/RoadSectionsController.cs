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
    /// Контролер для керування ділянками доріг.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoadSectionsController : ControllerBase
    {
        private readonly IRoadSectionService _roadSectionService;

        /// <summary>
        /// Конструктор контролера.
        /// </summary>
        /// <param name="roadSectionService">Сервіс для роботи з ділянками доріг.</param>
        public RoadSectionsController(IRoadSectionService roadSectionService)
        {
            _roadSectionService = roadSectionService;
        }

        /// <summary>
        /// Отримує список всіх ділянок доріг (доступно всім авторизованим).
        /// </summary>
        /// <returns>Список ділянок доріг.</returns>
        [HttpGet]
        [Authorize(Roles = "User, Operator, Admin")]
        public async Task<ActionResult<IEnumerable<RoadSection>>> GetRoadSections()
        {
            return Ok(await _roadSectionService.GetAllRoadSectionsAsync());
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
            var roadSection = await _roadSectionService.GetRoadSectionByIdAsync(id);
            if (roadSection == null)
            {
                return NotFound();
            }
            return Ok(roadSection);
        }

        /// <summary>
        /// Оновлює інформацію про ділянку дороги (тільки для Operator, Admin).
        /// </summary>
        /// <param name="id">ID ділянки.</param>
        /// <param name="roadSection">Об'єкт із новими даними.</param>
        /// <returns>Статус оновлення.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Operator, Admin")]
        public async Task<IActionResult> UpdateRoadSection(int id, RoadSectionDto roadSection)
        {
            var success = await _roadSectionService.UpdateRoadSectionAsync(id, roadSection);
            if (!success)
            {
                return NotFound();
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
        public async Task<ActionResult<RoadSection>> CreateRoadSection([FromBody] RoadSectionDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name) || string.IsNullOrWhiteSpace(createDto.Location))
            {
                return BadRequest("Name and Location are required.");
            }

            // Створення об'єкта RoadSection на основі DTO
            var roadSection = new RoadSection
            {
                Name = createDto.Name,
                Location = createDto.Location,
                CreatedDate = DateTime.UtcNow
            };

            var newSection = await _roadSectionService.CreateRoadSectionAsync(roadSection);
            return CreatedAtAction(nameof(GetRoadSection), new { id = newSection.RoadSectionID }, newSection);
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
            var success = await _roadSectionService.DeleteRoadSectionAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
