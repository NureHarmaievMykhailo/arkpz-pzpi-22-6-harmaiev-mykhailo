using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadMonitoringSystem.Models
{
    /// <summary>
    /// Представляє ділянку дороги.
    /// </summary>
    public class RoadSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoadSectionID { get; set; } // Унікальний ідентифікатор ділянки

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Назва ділянки

        [MaxLength(255)]
        public string Location { get; set; } // Географічні координати або опис

        [Required]
        public DateTime CreatedDate { get; set; } // Дата створення

        // Зв'язок із сенсорами
        public ICollection<Sensor> Sensors { get; set; } // Лист сенсорів для цієї ділянки
        public ICollection<Alert> Alerts { get; set; }

        public RoadSection()
        {
            Sensors = new List<Sensor>();
        }
    }
}
