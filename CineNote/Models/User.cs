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

        [Required(ErrorMessage = "Имя обязательно для заполнения.")]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен для заполнения.")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email.")]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен для заполнения.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль должен быть не менее 8 символов.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Нужно минимум 8 символов, хотя бы одна заглавная буква и цифра.")]
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Column("global_role")]
        public string GlobalRole { get; set; } = "Сценарист";
        public string? VerificationCode { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;

        public ICollection<MovieProject> MovieProjects { get; set; } = new List<MovieProject>();
        public ICollection<ProjectShare> ProjectShares { get; set; } = new List<ProjectShare>();
    }
}