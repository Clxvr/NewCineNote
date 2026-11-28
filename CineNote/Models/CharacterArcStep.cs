using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineNote.Models
{
    [Table("character_arc_steps")]
    public class CharacterArcStep
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("character_id")]
        public int CharacterId { get; set; }

        public Character? Character { get; set; }

        [Column("scene_id")]
        public int? SceneId { get; set; }

        public Scene? Scene { get; set; }

        [Column("act")]
        public MovieAct Act { get; set; } = MovieAct.Act1;

        [Required(ErrorMessage = "Укажите название этапа арки")]
        [StringLength(100)]
        [Column("stage_title")]
        public string StageTitle { get; set; } = string.Empty;

        [StringLength(500)]
        [Column("emotional_note")]
        public string EmotionalNote { get; set; } = string.Empty;

        [Column("order_index")]
        public int OrderIndex { get; set; }
    }
}
