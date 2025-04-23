using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRentalForms.Models
{
    [Table("User")]
    [Index("Email", Name = "UQ__User__A9D10534E3E0C641", IsUnique = true)]
    public partial class User
    {
        public User()
        {
            Documents = new HashSet<Document>();
            Feedbacks = new HashSet<Feedback>();
            Logs = new HashSet<Log>();
            Notifications = new HashSet<Notification>();
            RentalRequests = new HashSet<RentalRequest>();
            RentalTransactions = new HashSet<RentalTransaction>();
        }

        [Key]
        public int UserId { get; set; }
        [StringLength(100)]
        public string FullName { get; set; } = null!;
        [StringLength(100)]
        public string Email { get; set; } = null!;
        [StringLength(256)]
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("Users")]
        public virtual UserRole Role { get; set; } = null!;
        [InverseProperty("User")]
        public virtual ICollection<Document> Documents { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Log> Logs { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Notification> Notifications { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<RentalRequest> RentalRequests { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<RentalTransaction> RentalTransactions { get; set; }
    }
}
