using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRentalForms.Models
{
    [Table("RentalRequest")]
    public partial class RentalRequest
    {
        public RentalRequest()
        {
            RentalTransactions = new HashSet<RentalTransaction>();
        }

        [Key]
        public int RentalRequestId { get; set; }
        public int CustomerId { get; set; }
        public int EquipmentId { get; set; }
        [Column(TypeName = "date")]
        public DateTime RentalStartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime RentalEndDate { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalCost { get; set; }
        [StringLength(50)]
        public string Status { get; set; } = null!;
        [StringLength(255)]
        public string? Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("RentalRequests")]
        public virtual User Customer { get; set; } = null!;
        [ForeignKey("EquipmentId")]
        [InverseProperty("RentalRequests")]
        public virtual Equipment Equipment { get; set; } = null!;
        [InverseProperty("RentalRequest")]
        public virtual ICollection<RentalTransaction> RentalTransactions { get; set; }
    }
}
