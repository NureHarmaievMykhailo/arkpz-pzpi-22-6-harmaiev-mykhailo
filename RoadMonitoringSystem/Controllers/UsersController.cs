using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadMonitoringSystem.Data;
using RoadMonitoringSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadMonitoringSystem.Controllers
{
    /// <summary>
    /// Контролер для керування користувачами системи.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримує список всіх користувачів (доступно для User, Operator, Admin).
        /// </summary>
        [Authorize(Roles = "User,Operator,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Отримує користувача за ID (доступно для User, Operator, Admin).
        /// </summary>
        [Authorize(Roles = "User,Operator,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// Оновлює інформацію про користувача (доступно лише для Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.UserID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
        /// Додає нового користувача (доступно лише для Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }

        /// <summary>
        /// Видаляє користувача за ID (доступно лише для Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(u => u.UserID == id);
        }
    }
}