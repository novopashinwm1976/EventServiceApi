using System.ComponentModel.DataAnnotations;

namespace EventServiceApi.Data.DTO
{
    /// <summary>
    /// DTO для передачи данных о событии
    /// </summary>
    public class EventDto
    {
        /// <summary>
        /// Заголовок события
        /// </summary>
        [Required(ErrorMessage = "Заголовок события обязателен для заполнения.")]
        public required string Title { get; set; }
        
        /// <summary>
        /// Описание события
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата начала мероприятия
        /// </summary>
        [Required(ErrorMessage = "Дата начала мероприятия обязательно для заполнения.")]
        [Range(typeof(DateTime), "2020-01-01", "2030-12-31", ErrorMessage = "Некорректная дата")]
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Дата окончания мероприятия
        /// </summary>
        [Required(ErrorMessage = "Дата окончания мероприятия обязательно для заполнения.")]
        [Range(typeof(DateTime), "2020-01-01", "2030-12-31", ErrorMessage = "Некорректная дата")]
        public DateTime EndAt { get; set; }
    }
}
