﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DA.Models;

[Table("HOADON")]
public partial class Hoadon
{
    [Key]
    [Column("MaHD")]
    [Display(Name = "Mã hóa đơn")]
    public int MaHd { get; set; }

    [Display(Name = "Ngày")]
    [Column(TypeName = "datetime")]
    public DateTime? Ngay { get; set; }

    [Display(Name = "Tổng tiền")]
    public int? TongTien { get; set; }

    [Column("MaKH")]
    public int MaKh { get; set; }

    [Display(Name = "Trạng thái")]
    public int? TrangThai { get; set; }

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [ForeignKey("MaKh")]
    [InverseProperty("Hoadons")]
    [Display(Name = "Khách hàng")]
    public virtual Khachhang MaKhNavigation { get; set; } = null!;
}
