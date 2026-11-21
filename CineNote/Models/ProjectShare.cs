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

        public MovieProject? MovieProject { get; set; }

        [Required]
        [Column("shared_with_user_id")]
        public int SharedWithUserId { get; set; }

        [ForeignKey("SharedWithUserId")]
        public User? SharedWithUser { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("access_level")]
        public string AccessLevel { get; set; } = ProjectAccessLevel.Viewer;
    }

    public static class ProjectAccessLevel
    {
        public const string Owner = "Владелец";
        public const string Editor = "Редактор";
        public const string Viewer = "Читатель";

        public static string ToBadgeClass(string accessLevel) => accessLevel switch
        {
            Owner => "badge-access-owner",
            Editor => "badge-access-editor",
            Viewer => "badge-access-viewer",
            _ => "bg-secondary"
        };

        public static string ToIconClass(string accessLevel) => accessLevel switch
        {
            Owner => "bi-star-fill",
            Editor => "bi-pencil-fill",
            Viewer => "bi-eye-fill",
            _ => "bi-person-fill"
        };

        public static string ToDescription(string accessLevel) => accessLevel switch
        {
            Owner => "Полный доступ: управление проектом, соавторами и удаление",
            Editor => "Редактирование: изменение синопсиса, добавление и правка сцен, персонажей",
            Viewer => "Только чтение: ознакомление с материалами фильма без права внесения изменений",
            _ => string.Empty
        };
    }
}
