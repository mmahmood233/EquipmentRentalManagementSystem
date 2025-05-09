using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.DataAccess.Models
{
    public partial class Equipment
    {
        public Equipment()
        {
            Feedbacks = new HashSet<Feedback>();
            RentalRequests = new HashSet<RentalRequest>();
            RentalTransactions = new HashSet<RentalTransaction>();
        }

        [Key]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal RentalPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string AvailabilityStatus { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string ConditionStatus { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Equipment")]
        public virtual Category Category { get; set; } = null!;

        [InverseProperty("Equipment")]
        public virtual ICollection<Feedback> Feedbacks { get; set; }

        [InverseProperty("Equipment")]
        public virtual ICollection<RentalRequest> RentalRequests { get; set; }

        [InverseProperty("Equipment")]
        public virtual ICollection<RentalTransaction> RentalTransactions { get; set; }
    }
}
