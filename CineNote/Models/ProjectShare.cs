using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineNote.Models
{
    [Table("project_shares")]
    public class ProjectShare
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("movie_project_id")]
        public int MovieProjectId { get; set; }

        // Навигационное свойство на сам кинопроект
        public MovieProject? MovieProject { get; set; }

        [Required]
        [Column("shared_with_user_id")]
        public int SharedWithUserId { get; set; }

        // Навигационное свойство на пользователя, с которым поделились проектом
        // Указываем ForeignKey, так как имя свойства не совпадает напрямую с именем внешнего ключа в таблице
        [ForeignKey("SharedWithUserId")]
        public User? SharedWithUser { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("access_level")]
        public string AccessLevel { get; set; } = "Чтение";
    }
}