using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRentalForms.Models
{
    [Table("Notification")]
    public partial class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        [StringLength(255)]
        public string Message { get; set; } = null!;
        [StringLength(50)]
        public string Type { get; set; } = null!;
        public bool IsRead { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Notifications")]
        public virtual User User { get; set; } = null!;
    }
}
