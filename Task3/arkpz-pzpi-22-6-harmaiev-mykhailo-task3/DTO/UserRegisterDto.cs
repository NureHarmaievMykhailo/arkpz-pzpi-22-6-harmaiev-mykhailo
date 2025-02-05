using RoadMonitoringSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace RoadMonitoringSystem.DTO
{
    public class UserRegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public UserRoles? Role { get; set; }
    }
}
