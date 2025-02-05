using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadMonitoringSystem.Models
{
    /// <summary>
    /// Представляє дані, отримані від сенсора.
    /// </summary>
    public class SensorData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SensorDataID { get; set; } // Унікальний ідентифікатор даних

        [Required]
        public int SensorID { get; set; } // Зовнішній ключ до сенсора

        [Required]
        [MaxLength(50)]
        public string Parameter { get; set; } // Назва параметра (наприклад, "Температура", "Лід")

        [Required]
        public double DataValue { get; set; } // Значення параметра

        [Required]
        public DateTime Timestamp { get; set; } // Час і дата отримання даних

        // Навігаційна властивість для зв’язку з сенсором
        [ForeignKey("SensorID")]
        public Sensor Sensor { get; set; } // Власник даних
    }
}
