using System.ComponentModel.DataAnnotations;

namespace WebApiDemo_ML_lesson.Models.DTOs
{
    public class UserRegistrationDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
