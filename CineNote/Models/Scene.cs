using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineNote.Models
{
    [Table("scenes")]
    public class Scene
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("movie_project_id")]
        public int MovieProjectId { get; set; }

        [Required]
        [Column("scene_number")]
        public int SceneNumber { get; set; }

        [Column("location_type")]
        [StringLength(20)]
        public string LocationType { get; set; }

        [Column("location_name")]
        [StringLength(30)]
        public string LocationName { get; set; }

        [Column("time_of_day")]
        [StringLength(20)]
        public string TimeOfDay { get; set; }

        [Column("action_description")]
        public string ActionDescription { get; set; }
    }
}
