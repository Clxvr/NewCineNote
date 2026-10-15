using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineNote.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email обязателен для заполнения.")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
        [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов.")]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен для заполнения.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 100 символов.")]
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имя пользователя обязательно для заполнения.")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов.")]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("global_role")]
        public string GlobalRole { get; set; } = "Пользователь";

      
        public ICollection<MovieProject> MovieProjects { get; set; } = new List<MovieProject>();
        public ICollection<ProjectShare> ProjectShares { get; set; } = new List<ProjectShare>();
    }
}