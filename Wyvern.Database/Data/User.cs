using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wyvern.Database.Data
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(32)]
        [Column("username")]
        public string Username { get; set; } = null!;

        [Required]
        [Column("discriminator", TypeName = "char(4)")]
        public string Discriminator { get; set; } = "0000";

        [MaxLength(64)]
        [Column("display_name")]
        public string? DisplayName { get; set; }

        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("birthday")]
        public DateTime Birthday { get; set; }

        [Column("token_h")]
        public string TokenHash { get; set; } = null!;

        [Column("token_e")]
        public string TokenEncrypted { get; set; } = null!;

        [Column("avatar")]
        public string? Avatar { get; set; }

        [Column("banner")]
        public string? Banner { get; set; }

        [Column("about")]
        public string? About { get; set; }

        [MaxLength(10)]
        [Column("locale")]
        public string Locale { get; set; } = null!;

        [MaxLength(10)]
        [Column("region")]
        public string Region { get; set; } = null!;

        [Column("rank")]
        public long Rank { get; set; } = 0;

        [Column("badges")]
        public long Badges { get; set; } = 0;

        [Column("is_bot")]
        public bool IsBot { get; set; } = false;

        [Column("is_staff")]
        public bool IsStaff { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
