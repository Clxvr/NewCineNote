using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineNote.Models
{
    [Table("contact_feedbacks")]
    public class ContactFeedback
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("subject")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Column("message")]
        public string Message { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_resolved")]
        public bool IsResolved { get; set; } = false;
    }
}
