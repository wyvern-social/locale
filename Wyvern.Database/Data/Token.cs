using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wyvern.Utils.Generators;

namespace Wyvern.Database.Data
{
    [Table("tokens")]
    public class Token
    {
        [Key]
        [Column("id", TypeName = "char(26)")]
        public string Id { get; set; } = IdGen.GenerateId();

        [Required]
        [Column("user_id", TypeName = "char(26)")]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        [Column("token")]
        public string TokenValue { get; set; } = TokenGen.GenerateToken(30);

        [Required]
        [MaxLength(32)]
        [Column("type")]
        public string Type { get; set; } = "email_verification";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("used")]
        public bool Used { get; set; } = false;
    }
}
