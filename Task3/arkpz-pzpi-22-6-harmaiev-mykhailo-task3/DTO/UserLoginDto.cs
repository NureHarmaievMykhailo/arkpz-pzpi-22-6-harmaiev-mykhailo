using System.ComponentModel.DataAnnotations;

namespace RoadMonitoringSystem.DTO
{
    public class UserLoginDto
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
