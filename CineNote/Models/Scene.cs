using CineNote.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CineNote.Models
{
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
        public LocationType LocationType { get; set; } = LocationType.INT;

        [Column("location_name")]
        [StringLength(30)]
        public string LocationName { get; set; } = string.Empty;

        [Column("time_of_day")]
        public TimeOfDay TimeOfDay { get; set; } = TimeOfDay.Day;

        [Column("action_description")]
        public string ActionDescription { get; set; } = string.Empty;

        [NotMapped]
        public string Slugline => $"{LocationType.ToDisplayName()} {LocationName} - {TimeOfDay.ToDisplayName()}".ToUpper();

        public ICollection<SceneCharacter> SceneCharacters { get; set; } = new List<SceneCharacter>();
    }
}