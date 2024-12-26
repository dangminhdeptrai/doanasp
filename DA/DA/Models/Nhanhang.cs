using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DA.Models;

[Table("NHANHANG")]
public partial class Nhanhang
{
    [Key]
    [Column("MaNH")]
    public int MaNh { get; set; }

    [Required(ErrorMessage = "Chưa nhập tên nhãn hàng")]
    [Display(Name = "Tên nhãn hàng")]
	[StringLength(100)]
    public string Ten { get; set; } = null!;

    [InverseProperty("MaNhNavigation")]
    public virtual ICollection<Mathang> Mathangs { get; set; } = new List<Mathang>();
}
