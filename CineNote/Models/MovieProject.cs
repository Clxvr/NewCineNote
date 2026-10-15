using CineNote.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class MovieProject
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Название кинопроекта является обязательным полем.")]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    // Навигационное свойство на создателя проекта:
    public User? User { get; set; }

    [Required(ErrorMessage = "Укажите жанр.")]
    [Column("genre")]
    public Genre Genre { get; set; }

    [StringLength(500, ErrorMessage = "Логлайн не должен превышать 500 символов.")]
    [Column("logline")]
    public string Logline { get; set; } = string.Empty;

    [StringLength(5000, ErrorMessage = "Синопсис не должен превышать 5000 символов.")]
    [Column("synopsis")]
    public string Synopsis { get; set; } = string.Empty;

    // Коллекции для связи «один ко многим»:
    public ICollection<Character> Characters { get; set; } = new List<Character>();
    public ICollection<Scene> Scenes { get; set; } = new List<Scene>();
    public ICollection<ProjectShare> ProjectShares { get; set; } = new List<ProjectShare>();
}