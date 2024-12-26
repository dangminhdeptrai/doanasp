using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DA.Models;

[Table("MATHANG")]
public partial class Mathang
{
    [Key]
    [Column("MaMH")]
    public int MaMh { get; set; }

    [Display(Name = "Tên mặt hàng")]
    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [DisplayFormat(DataFormatString = "{0:#,##0} đ")]
    [Display(Name = "Giá gốc")]
    public int? GiaGoc { get; set; }

    [DisplayFormat(DataFormatString = "{0:#,##0} đ")]
    [Display(Name = "Giá bán")]
    public int? GiaBan { get; set; }

    [Display(Name = "Số lượng")]
    public short? SoLuong { get; set; }

    [Display(Name = "Mô tả")]
    [StringLength(1000)]
    public string? MoTa { get; set; }

    [Display(Name = "Hình ảnh")]
    [StringLength(255)]
    [Unicode(false)]
    public string? HinhAnh { get; set; }

    [Column("MaNH")]
    [Display(Name = "Nhãn hàng")]
    public int MaNh { get; set; }

    [Display(Name = "Lượt xem")]
    public int? LuotXem { get; set; }

    [Display(Name = "Lượt mua")]
    public int? LuotMua { get; set; }

    [InverseProperty("MaMhNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [Display(Name = "Nhãn hàng")]
    [ForeignKey("MaNh")]
    [InverseProperty("Mathangs")]
    public virtual Nhanhang MaNhNavigation { get; set; } = null!;

}
