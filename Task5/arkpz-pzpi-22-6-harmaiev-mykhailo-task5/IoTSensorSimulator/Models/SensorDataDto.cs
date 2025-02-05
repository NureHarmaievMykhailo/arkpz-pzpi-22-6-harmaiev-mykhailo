using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IoTSensorSimulator.Models
{
    /// <summary>
    /// DTO для даних сенсора, що передаються на сервер.
    /// </summary>
    public class SensorDataDto
    {
        /// <summary>
        /// Унікальний ідентифікатор сенсора.
        /// </summary>
        [Required]
        public int SensorID { get; set; }

        /// <summary>
        /// Параметр, який вимірюється (наприклад, "Температура", "Вологість", "Ямковість" або "Лід").
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Параметр не може перевищувати 50 символів.")]
        public string Parameter { get; set; }

        /// <summary>
        /// Значення виміряного параметра.
        /// </summary>
        [Required]
        public float DataValue { get; set; }

        /// <summary>
        /// Дата та час отримання даних.
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
