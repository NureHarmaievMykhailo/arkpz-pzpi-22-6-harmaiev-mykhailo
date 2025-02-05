using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RoadMonitoringSystem.Models
{
    /// <summary>
    /// Представляє сповіщення про критичну ситуацію на дорозі.
    /// </summary>
    public class Alert
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlertID { get; set; } // Унікальний ідентифікатор сповіщення

        [Required]
        public int RoadSectionID { get; set; } // Зовнішній ключ до RoadSection

        [Required]
        [MaxLength(50)]
        public string AlertType { get; set; } // Тип сповіщення (наприклад, "Лід", "Яма")

        [Required]
        [MaxLength(255)]
        public string Message { get; set; } // Опис проблеми

        [Required]
        public DateTime CreatedDate { get; set; } // Дата створення сповіщення

        [Required]
        public bool IsResolved { get; set; } // Чи вирішено проблему (true/false)

        // Навігаційна властивість для зв’язку з RoadSection
        [ForeignKey("RoadSectionID")]
        public RoadSection RoadSection { get; set; } // Ділянка, до якої належить сповіщення

        //[JsonIgnore] // Запобігає циклічній серіалізації
        //public RoadSection RoadSection { get; set; }
    }
}
