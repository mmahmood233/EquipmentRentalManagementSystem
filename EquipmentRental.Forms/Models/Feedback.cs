using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.DataAccess.Models
{
    [Table("Feedback")]
    public partial class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public int EquipmentId { get; set; }
        public int UserId { get; set; }
        [StringLength(255)]
        public string? CommentText { get; set; }
        public int? Rating { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Required]
        public bool? IsVisible { get; set; }

        [ForeignKey("EquipmentId")]
        [InverseProperty("Feedbacks")]
        public virtual Equipment Equipment { get; set; } = null!;
        [ForeignKey("UserId")]
        [InverseProperty("Feedbacks")]
        public virtual User User { get; set; } = null!;
    }
}
