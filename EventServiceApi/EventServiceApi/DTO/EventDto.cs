using System.ComponentModel.DataAnnotations;

namespace EventServiceApi.DTO
{
    public class EventDto
    {
        [Required(ErrorMessage = "Заголовок события обязателен для заполнения.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Дата начала мероприятия обязательно для заполнения.")]
        [Range(typeof(DateTime), "2020-01-01", "2030-12-31", ErrorMessage = "Некорректная дата")]
        public DateTime StartAt { get; set; }

        [Required(ErrorMessage = "Дата окончания мероприятия обяально для заполнения.")]
        [Range(typeof(DateTime), "2020-01-01", "2030-12-31", ErrorMessage = "Некорректная дата")]
        public DateTime EndAt { get; set; }
    }
}
