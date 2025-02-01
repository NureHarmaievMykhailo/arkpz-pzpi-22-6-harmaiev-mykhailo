using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<Alert> Alerts { get; set; }

        //[JsonIgnore]
        //public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();

        //[JsonIgnore]
        //public ICollection<Alert> Alerts { get; set; } = new List<Alert>();

        public RoadSection()
        {
            Sensors = new List<Sensor>();
        }

        
    }
}
