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
    /// Контролер для керування користувачами системи.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Отримує список всіх користувачів (доступно для User, Operator, Admin).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "User,Operator,Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        /// <summary>
        /// Отримує користувача за ID (доступно для User, Operator, Admin).
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Operator,Admin")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Реєструє нового користувача з хешованим паролем.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register([FromBody] UserRegisterDto userDto)
        {
            // Перевіряємо, чи це адміністратор
            bool isAdmin = User.IsInRole(UserRoles.Admin.ToString());

            var user = await _userService.RegisterUserAsync(userDto, isAdmin);
            if (user == null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.UserID }, user);
        }


        /// <summary>
        /// Авторизує користувача і повертає JWT-токен.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var token = await _userService.AuthenticateUserAsync(loginDto);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            return Ok(new { token });
        }

        /// <summary>
        /// Оновлює роль користувача (доступно лише для Admin).
        /// </summary>
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UserRoles newRole)
        {
            var success = await _userService.UpdateUserRoleAsync(id, newRole);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Видаляє користувача за ID (доступно лише для Admin).
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
