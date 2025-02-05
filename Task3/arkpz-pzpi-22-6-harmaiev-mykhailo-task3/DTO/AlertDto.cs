using System.ComponentModel.DataAnnotations;

namespace RoadMonitoringSystem.DTO
{
    public class AlertDto
    {
        [Required]
        public int RoadSectionID { get; set; }

        [Required]
        [MaxLength(50)]
        public string AlertType { get; set; }

        [Required]
        [MaxLength(255)]
        public string Message { get; set; }
    }
}
