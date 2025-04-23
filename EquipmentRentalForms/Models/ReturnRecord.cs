using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRentalForms.Models
{
    [Table("ReturnRecord")]
    public partial class ReturnRecord
    {
        [Key]
        public int ReturnRecordId { get; set; }
        public int RentalTransactionId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ActualReturnDate { get; set; }
        [StringLength(50)]
        public string ReturnCondition { get; set; } = null!;
        [Column(TypeName = "decimal(10, 2)")]
        public decimal LateReturnFee { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal AdditionalCharges { get; set; }
        [StringLength(255)]
        public string? Notes { get; set; }

        [ForeignKey("RentalTransactionId")]
        [InverseProperty("ReturnRecords")]
        public virtual RentalTransaction RentalTransaction { get; set; } = null!;
    }
}
