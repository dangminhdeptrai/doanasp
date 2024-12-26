using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DA.Models;

[Table("NHANVIEN")]
public partial class Nhanvien
{
    [Key]
    [Column("MaNV")]
    public int MaNv { get; set; }

    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? DienThoai { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? MatKhau { get; set; }
}
