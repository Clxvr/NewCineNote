using CineNote.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Character
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("movie_project_id")]
    public int MovieProjectId { get; set; }

    public MovieProject? MovieProject { get; set; }

    [Required(ErrorMessage = "Имя персонажа обязательно для заполнения.")]
    [Column("name")]
    [StringLength(30, ErrorMessage = "Имя героя не может превышать лимит в 30 символов.")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column("role")]
    public CharacterRole Role { get; set; } = CharacterRole.Supporting;

    public ICollection<SceneCharacter> SceneCharacters { get; set; } = new List<SceneCharacter>();
}