using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.DataAccess.Models
{
    [Table("Category")]
    [Index("CategoryName", Name = "UQ__Category__8517B2E0B3CCEAFF", IsUnique = true)]
    public partial class Category
    {
        public Category()
        {
            Equipment = new HashSet<Equipment>();
        }

        [Key]
        public int CategoryId { get; set; }
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;
        [StringLength(255)]
        public string? Description { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Equipment> Equipment { get; set; }
        
        [NotMapped]
        public int EquipmentCount { get; set; }
    }
}
