using CineNote.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class MovieProject
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Название кинопроекта является обязательным полем.")]
    [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов.")]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required(ErrorMessage = "Укажите жанр.")]
    [Column("genre")]
    public Genre Genre { get; set; }

    [StringLength(250, ErrorMessage = "Логлайн не должен превышать 250 символов.")]
    [Column("logline")]
    public string Logline { get; set; } = string.Empty;

    [StringLength(5000, ErrorMessage = "Синопсис не должен превышать 5000 символов.")]
    [Column("synopsis")]
    public string Synopsis { get; set; } = string.Empty;

    public ICollection<Character> Characters { get; set; } = new List<Character>();
    public ICollection<Scene> Scenes { get; set; } = new List<Scene>();
    public ICollection<ProjectShare> ProjectShares { get; set; } = new List<ProjectShare>();
}