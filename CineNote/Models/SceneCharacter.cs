using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CineNote.Models;

public class SceneCharacter
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("scene_id")]
    public int SceneId { get; set; }
    public Scene? Scene { get; set; }

    [Required]
    [Column("character_id")]
    public int CharacterId { get; set; }
    public Character? Character { get; set; }
}