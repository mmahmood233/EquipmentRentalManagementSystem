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

        [Required]
        public int EquipmentId { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string CommentText { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        [NotMapped]
        public virtual Equipment? Equipment { get; set; }

        [NotMapped]
        public virtual User? User { get; set; }

    }
}
