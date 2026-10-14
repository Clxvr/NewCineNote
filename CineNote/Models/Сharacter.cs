using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineNote.Models
{
    [Table("characters")]
    public class Character
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("movie_project_id")]
        public int MovieProjectId { get; set; }

        [Required(ErrorMessage = "Имя персонажа обязательно для заполнения.")]
        [Column("name")]
        [StringLength(30, ErrorMessage = "Имя героя не может превышать лимит в 30 символов.")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }
}
