 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineNote.Models
{
    [Table("scene_characters")]
    public class SceneCharacter
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("scene_id")]
        public int SceneId { get; set; }

        [Required]
        [Column("character_id")]
        public int CharacterId { get; set; }
    }
}