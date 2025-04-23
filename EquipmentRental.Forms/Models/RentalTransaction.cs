using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.DataAccess.Models
{
    [Table("RentalTransaction")]
    public partial class RentalTransaction
    {
        public RentalTransaction()
        {
            Documents = new HashSet<Document>();
            ReturnRecords = new HashSet<ReturnRecord>();
        }

        [Key]
        public int RentalTransactionId { get; set; }
        public int RentalRequestId { get; set; }
        public int EquipmentId { get; set; }
        public int CustomerId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RentalStartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RentalEndDate { get; set; }
        public int RentalPeriod { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal RentalFee { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Deposit { get; set; }
        [StringLength(50)]
        public string PaymentStatus { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("RentalTransactions")]
        public virtual User Customer { get; set; } = null!;
        [ForeignKey("EquipmentId")]
        [InverseProperty("RentalTransactions")]
        public virtual Equipment Equipment { get; set; } = null!;
        [ForeignKey("RentalRequestId")]
        [InverseProperty("RentalTransactions")]
        public virtual RentalRequest RentalRequest { get; set; } = null!;
        [InverseProperty("RentalTransaction")]
        public virtual ICollection<Document> Documents { get; set; }
        [InverseProperty("RentalTransaction")]
        public virtual ICollection<ReturnRecord> ReturnRecords { get; set; }
    }
}
