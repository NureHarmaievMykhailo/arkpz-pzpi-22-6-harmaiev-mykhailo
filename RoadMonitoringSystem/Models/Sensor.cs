using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadMonitoringSystem.Models
{
    /// <summary>
    /// Представляє сенсор, встановлений на ділянці дороги.
    /// </summary>
    public class Sensor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SensorID { get; set; } // Унікальний ідентифікатор сенсора

        [Required]
        public int RoadSectionID { get; set; } // Зовнішній ключ до RoadSection

        [Required]
        [MaxLength(50)]
        public string SensorType { get; set; } // Тип сенсора (наприклад, "Температура", "Лід")

        [Required]
        public DateTime InstallationDate { get; set; } // Дата встановлення сенсора

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } // Статус сенсора (наприклад, "Активний", "Не працює")

        // Навігаційна властивість для зв’язку з RoadSection
        [ForeignKey("RoadSectionID")]
        public RoadSection RoadSection { get; set; } // Власник сенсора

        public ICollection<SensorData> SensorData { get; set; }

        public ICollection<Alert> Alerts { get; set; }
    }
}
