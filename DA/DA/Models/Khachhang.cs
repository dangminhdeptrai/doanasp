using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DA.Models;

[Table("KHACHHANG")]
public partial class Khachhang
{
    [Key]
    [Column("MaKH")]
    public int MaKh { get; set; }

    [Display(Name = "Tên")]
    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [Display(Name = "Số điện thoại")]
    [StringLength(20)]
    [Unicode(false)]
    public string? DienThoai { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Display(Name = "Mật khẩu")]
    [StringLength(255)]
    [Unicode(false)]
    public string? MatKhau { get; set; }

    [InverseProperty("MaKhNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
}
