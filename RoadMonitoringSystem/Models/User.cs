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
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        public UserRoles Role { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
