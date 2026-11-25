using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineNote.Models
{
    [Table("scene_props")]
    public class SceneProp
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("scene_id")]
        public int SceneId { get; set; }

        public Scene? Scene { get; set; }

        [Required(ErrorMessage = "Укажите название реквизита или ресурса")]
        [StringLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("category")]
        public PropCategory Category { get; set; } = PropCategory.Props;

        [Range(1, 999)]
        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        [StringLength(200)]
        [Column("notes")]
        public string Notes { get; set; } = string.Empty;
    }
}
