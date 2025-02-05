using Microsoft.EntityFrameworkCore;
using RoadMonitoringSystem.Data;
using RoadMonitoringSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using RoadMonitoringSystem.DTO;

namespace RoadMonitoringSystem.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> RegisterUserAsync(UserRegisterDto userDto, bool isAdmin = false);
        Task<string?> AuthenticateUserAsync(UserLoginDto loginDto);
        Task<bool> UpdateUserRoleAsync(int id, UserRoles newRole);
        Task<bool> DeleteUserAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public UserService(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Реєстрація нового користувача з хешуванням пароля через BCrypt.
        /// </summary>
        public async Task<User?> RegisterUserAsync(UserRegisterDto userDto, bool isAdmin = false)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return null; // Користувач із таким ім'ям вже існує
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = hashedPassword,
                Role = isAdmin && userDto.Role.HasValue ? userDto.Role.Value : UserRoles.User, // Якщо адміністратор – дозволяємо вказати роль
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }


        /// <summary>
        /// Авторизація користувача через перевірку пароля у BCrypt.
        /// </summary>
        public async Task<string?> AuthenticateUserAsync(UserLoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            return _jwtService.GenerateToken(user);
        }

        /// <summary>
        /// Оновлення ролі користувача.
        /// </summary>
        public async Task<bool> UpdateUserRoleAsync(int id, UserRoles newRole)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            user.Role = newRole;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Видалення користувача.
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
