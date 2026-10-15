using CineNote.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Scene
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("movie_project_id")]
    public int MovieProjectId { get; set; }

    public MovieProject? MovieProject { get; set; }

    [Required]
    [Column("scene_number")]
    public int SceneNumber { get; set; }

    [Column("location_type")]
    [StringLength(20)]
    public string LocationType { get; set; } = string.Empty;

    [Column("location_name")]
    [StringLength(30)]
    public string LocationName { get; set; } = string.Empty;

    [Column("time_of_day")]
    [StringLength(20)]
    public string TimeOfDay { get; set; } = string.Empty;

    [Column("action_description")]
    public string ActionDescription { get; set; } = string.Empty;

    public ICollection<SceneCharacter> SceneCharacters { get; set; } = new List<SceneCharacter>();
}