using System;
using System.Collections.Generic;
using DA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DA.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cthoadon> Cthoadons { get; set; }

    public virtual DbSet<Cuahang> Cuahangs { get; set; }

    public virtual DbSet<Hoadon> Hoadons { get; set; }

    public virtual DbSet<Khachhang> Khachhangs { get; set; }

    public virtual DbSet<Mathang> Mathangs { get; set; }

    public virtual DbSet<Nhanhang> Nhanhangs { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-U2U4NTC\\SQLEXPRESS;Initial Catalog=shopgiay;User ID=sa;Password=sql2017;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cthoadon>(entity =>
        {
            entity.HasKey(e => e.MaCthd).HasName("PK__CTHOADON__1E4FA7714E8F85CC");

            entity.Property(e => e.DonGia).HasDefaultValue(0);
            entity.Property(e => e.SoLuong).HasDefaultValue((short)1);

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.Cthoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHOADON__MaHD__4CA06362");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.Cthoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHOADON__MaMH__4D94879B");
        });

        modelBuilder.Entity<Cuahang>(entity =>
        {
            entity.HasKey(e => e.MaCh).HasName("PK__CUAHANG__27258E000E2DA572");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HOADON__2725A6E0CE93670F");

            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue(0);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Hoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HOADON__MaKH__48CFD27E");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KHACHHAN__2725CF1EB9259666");
        });

        modelBuilder.Entity<Mathang>(entity =>
        {
            entity.HasKey(e => e.MaMh).HasName("PK__MATHANG__2725DFD915733DE4");

            entity.Property(e => e.GiaBan).HasDefaultValue(0);
            entity.Property(e => e.GiaGoc).HasDefaultValue(0);
            entity.Property(e => e.LuotMua).HasDefaultValue(0);
            entity.Property(e => e.LuotXem).HasDefaultValue(0);
            entity.Property(e => e.SoLuong).HasDefaultValue((short)0);

            entity.HasOne(d => d.MaNhNavigation).WithMany(p => p.Mathangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MATHANG__MaNH__3E52440B");
        });

        modelBuilder.Entity<Nhanhang>(entity =>
        {
            entity.HasKey(e => e.MaNh).HasName("PK__NHANHANG__2725D738308BFCAE");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NHANVIEN__2725D70AEEA935F2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
