using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineNote.Models
{
    [Table("movie_projects")]
    public class MovieProject
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название кинопроекта является обязательным полем.")]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Укажите жанр.")]
    [Column("genre")]
    public Genre Genre { get; set; }

    [StringLength(500, ErrorMessage = "Логлайн не должен превышать 500 символов.")]
    [Column("logline")]
    public string Logline { get; set; }

    [StringLength(5000, ErrorMessage = "Синопсис не должен превышать 5000 символов.")]
    [Column("synopsis")]
    public string Synopsis { get; set; }    
    }
}