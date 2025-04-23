using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.DataAccess.Models
{
    [Table("Document")]
    public partial class Document
    {
        [Key]
        public int DocumentId { get; set; }
        public int? RentalTransactionId { get; set; }
        public int? UserId { get; set; }
        [StringLength(255)]
        public string FileName { get; set; } = null!;
        [StringLength(50)]
        public string FileType { get; set; } = null!;
        [StringLength(255)]
        public string StoragePath { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime UploadedAt { get; set; }

        [ForeignKey("RentalTransactionId")]
        [InverseProperty("Documents")]
        public virtual RentalTransaction? RentalTransaction { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Documents")]
        public virtual User? User { get; set; }
    }
}
