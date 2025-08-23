using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wyvern.Database.Data
{
    [Table("waitlist")]
    public class Waitlist
    {
        [Key]
        [MaxLength(32)]
        [Column("username")]
        public string Username { get; set; } = null!;

        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = null!;
    }
}
