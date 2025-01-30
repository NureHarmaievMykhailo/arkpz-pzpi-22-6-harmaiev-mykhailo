using System;
using System.ComponentModel.DataAnnotations;

namespace RoadMonitoringSystem.Models
{
    /// <summary>
    /// Представляє користувача системи.
    /// </summary>
    public class User
    {
        [Key]
        [Required]
        public int UserID { get; set; } // Унікальний ідентифікатор користувача

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } // Ім'я користувача (логін)

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } // Хеш пароля

        [Required]
        public UserRoles Role { get; set; } // Роль користувача (наприклад, "Адміністратор", "Працівник")

        [Required]
        public DateTime CreatedDate { get; set; } // Дата створення облікового запису
    }
}
