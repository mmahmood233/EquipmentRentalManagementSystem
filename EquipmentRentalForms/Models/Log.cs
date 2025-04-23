﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRentalForms.Models
{
    [Table("Log")]
    public partial class Log
    {
        [Key]
        public int LogId { get; set; }
        public int? UserId { get; set; }
        [StringLength(255)]
        public string Action { get; set; } = null!;
        [StringLength(255)]
        public string? Exception { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
        [StringLength(50)]
        public string Source { get; set; } = null!;

        [ForeignKey("UserId")]
        [InverseProperty("Logs")]
        public virtual User? User { get; set; }
    }
}
