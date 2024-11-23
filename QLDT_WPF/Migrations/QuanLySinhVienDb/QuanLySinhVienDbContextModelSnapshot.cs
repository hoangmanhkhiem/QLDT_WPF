﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QLDT_WPF.Data;

#nullable disable

namespace QLDT_WPF.Migrations.QuanLySinhVienDb
{
    [DbContext(typeof(QuanLySinhVienDbContext))]
    partial class QuanLySinhVienDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QLDT_WPF.Models.ChuongTrinhHoc", b =>
                {
                    b.Property<string>("IdChuongTrinhHoc")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("TenChuongTrinhHoc")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdChuongTrinhHoc");

                    b.ToTable("ChuongTrinhHoc", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.ChuongTrinhHocMonHoc", b =>
                {
                    b.Property<string>("IdCthmonHoc")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("IdCTHMonHoc")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("IdChuongTrinhHoc")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdMonHoc")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdCthmonHoc");

                    b.HasIndex("IdChuongTrinhHoc");

                    b.HasIndex("IdMonHoc");

                    b.ToTable("ChuongTrinhHoc_MonHoc", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.DangKyDoiLich", b =>
                {
                    b.Property<string>("IdDangKyDoiLich")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("IdThoiGian")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("ThoiGianBatDauHienTai")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("ThoiGianBatDauMoi")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ThoiGianKetThucHienTai")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("ThoiGianKetThucMoi")
                        .HasColumnType("datetime");

                    b.Property<int>("TrangThai")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(-1);

                    b.HasKey("IdDangKyDoiLich");

                    b.HasIndex("IdThoiGian");

                    b.ToTable("DangKyDoiLich", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.DangKyNguyenVong", b =>
                {
                    b.Property<string>("IdDangKyNguyenVong")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("IdMonHoc")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdSinhVien")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("TrangThai")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(-1);

                    b.HasKey("IdDangKyNguyenVong");

                    b.HasIndex("IdMonHoc");

                    b.HasIndex("IdSinhVien");

                    b.ToTable("DangKyNguyenVong", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.Diem", b =>
                {
                    b.Property<string>("IdDiem")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasDefaultValueSql("(newid())");

                    b.Property<decimal?>("DiemKetThuc")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal?>("DiemQuaTrinh")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal?>("DiemTongKet")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<string>("IdLopHocPhan")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdSinhVien")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("LanHoc")
                        .HasColumnType("int");

                    b.HasKey("IdDiem");

                    b.HasIndex("IdLopHocPhan");

                    b.HasIndex("IdSinhVien");

                    b.ToTable("Diem", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.GiaoVien", b =>
                {
                    b.Property<string>("IdGiaoVien")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdKhoa")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SoDienThoai")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("TenGiaoVien")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdGiaoVien");

                    b.HasIndex("IdKhoa");

                    b.ToTable("GiaoVien", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.Khoa", b =>
                {
                    b.Property<string>("IdKhoa")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TenKhoa")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdKhoa");

                    b.ToTable("Khoa", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.LopHocPhan", b =>
                {
                    b.Property<string>("IdLopHocPhan")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdGiaoVien")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdMonHoc")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TenHocPhan")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("ThoiGianBatDau")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ThoiGianKetThuc")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("TrangThaiNhapDiem")
                        .HasColumnType("bit");

                    b.HasKey("IdLopHocPhan");

                    b.HasIndex("IdGiaoVien");

                    b.HasIndex("IdMonHoc");

                    b.ToTable("LopHocPhan", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.MonHoc", b =>
                {
                    b.Property<string>("IdMonHoc")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdKhoa")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("SoTietHoc")
                        .HasColumnType("int");

                    b.Property<int?>("SoTinChi")
                        .HasColumnType("int");

                    b.Property<string>("TenMonHoc")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdMonHoc");

                    b.HasIndex("IdKhoa");

                    b.ToTable("MonHoc", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.SinhVien", b =>
                {
                    b.Property<string>("IdSinhVien")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("DiaChi")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdChuongTrinhHoc")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdKhoa")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Lop")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("NgaySinh")
                        .HasColumnType("date");

                    b.HasKey("IdSinhVien");

                    b.HasIndex("IdChuongTrinhHoc");

                    b.HasIndex("IdKhoa");

                    b.ToTable("SinhVien", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.SinhVienLopHocPhan", b =>
                {
                    b.Property<string>("IdSinhVienLopHp")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("IdSinhVienLopHP")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("IdLopHocPhan")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdSinhVien")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdSinhVienLopHp");

                    b.HasIndex("IdLopHocPhan");

                    b.HasIndex("IdSinhVien");

                    b.ToTable("SinhVien_LopHocPhan", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.ThoiGian", b =>
                {
                    b.Property<string>("IdThoiGian")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("DiaDiem")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("NgayBatDau")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("NgayKetThuc")
                        .HasColumnType("datetime");

                    b.HasKey("IdThoiGian");

                    b.ToTable("ThoiGian", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.ThoiGianLopHocPhan", b =>
                {
                    b.Property<string>("IdThoigianLopHp")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("IdThoigianLopHP")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("IdLopHocPhan")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("IdThoiGian")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdThoigianLopHp");

                    b.HasIndex("IdLopHocPhan");

                    b.HasIndex("IdThoiGian");

                    b.ToTable("ThoiGian_LopHocPhan", (string)null);
                });

            modelBuilder.Entity("QLDT_WPF.Models.ChuongTrinhHocMonHoc", b =>
                {
                    b.HasOne("QLDT_WPF.Models.ChuongTrinhHoc", "ChuongTrinhHocs")
                        .WithMany("ChuongTrinhHocMonHocs")
                        .HasForeignKey("IdChuongTrinhHoc");

                    b.HasOne("QLDT_WPF.Models.MonHoc", "MonHocs")
                        .WithMany("ChuongTrinhHocMonHocs")
                        .HasForeignKey("IdMonHoc");

                    b.Navigation("ChuongTrinhHocs");

                    b.Navigation("MonHocs");
                });

            modelBuilder.Entity("QLDT_WPF.Models.DangKyDoiLich", b =>
                {
                    b.HasOne("QLDT_WPF.Models.ThoiGian", "ThoiGians")
                        .WithMany("DangKyDoiLichs")
                        .HasForeignKey("IdThoiGian")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ThoiGians");
                });

            modelBuilder.Entity("QLDT_WPF.Models.DangKyNguyenVong", b =>
                {
                    b.HasOne("QLDT_WPF.Models.MonHoc", "MonHocs")
                        .WithMany("DangKyNguyenVongs")
                        .HasForeignKey("IdMonHoc")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QLDT_WPF.Models.SinhVien", "SinhViens")
                        .WithMany("DangKyNguyenVongs")
                        .HasForeignKey("IdSinhVien")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MonHocs");

                    b.Navigation("SinhViens");
                });

            modelBuilder.Entity("QLDT_WPF.Models.Diem", b =>
                {
                    b.HasOne("QLDT_WPF.Models.LopHocPhan", "LopHocPhans")
                        .WithMany("Diems")
                        .HasForeignKey("IdLopHocPhan");

                    b.HasOne("QLDT_WPF.Models.SinhVien", "SinhViens")
                        .WithMany("Diems")
                        .HasForeignKey("IdSinhVien");

                    b.Navigation("LopHocPhans");

                    b.Navigation("SinhViens");
                });

            modelBuilder.Entity("QLDT_WPF.Models.GiaoVien", b =>
                {
                    b.HasOne("QLDT_WPF.Models.Khoa", "Khoas")
                        .WithMany("GiaoViens")
                        .HasForeignKey("IdKhoa");

                    b.Navigation("Khoas");
                });

            modelBuilder.Entity("QLDT_WPF.Models.LopHocPhan", b =>
                {
                    b.HasOne("QLDT_WPF.Models.GiaoVien", "GiaoViens")
                        .WithMany("LopHocPhans")
                        .HasForeignKey("IdGiaoVien");

                    b.HasOne("QLDT_WPF.Models.MonHoc", "MonHocs")
                        .WithMany("LopHocPhans")
                        .HasForeignKey("IdMonHoc");

                    b.Navigation("GiaoViens");

                    b.Navigation("MonHocs");
                });

            modelBuilder.Entity("QLDT_WPF.Models.MonHoc", b =>
                {
                    b.HasOne("QLDT_WPF.Models.Khoa", "Khoas")
                        .WithMany("MonHocs")
                        .HasForeignKey("IdKhoa");

                    b.Navigation("Khoas");
                });

            modelBuilder.Entity("QLDT_WPF.Models.SinhVien", b =>
                {
                    b.HasOne("QLDT_WPF.Models.ChuongTrinhHoc", "ChuongTrinhHocs")
                        .WithMany("SinhViens")
                        .HasForeignKey("IdChuongTrinhHoc");

                    b.HasOne("QLDT_WPF.Models.Khoa", "Khoas")
                        .WithMany("SinhViens")
                        .HasForeignKey("IdKhoa");

                    b.Navigation("ChuongTrinhHocs");

                    b.Navigation("Khoas");
                });

            modelBuilder.Entity("QLDT_WPF.Models.SinhVienLopHocPhan", b =>
                {
                    b.HasOne("QLDT_WPF.Models.LopHocPhan", "LopHocPhans")
                        .WithMany("SinhVienLopHocPhans")
                        .HasForeignKey("IdLopHocPhan");

                    b.HasOne("QLDT_WPF.Models.SinhVien", "SinhViens")
                        .WithMany("SinhVienLopHocPhans")
                        .HasForeignKey("IdSinhVien");

                    b.Navigation("LopHocPhans");

                    b.Navigation("SinhViens");
                });

            modelBuilder.Entity("QLDT_WPF.Models.ThoiGianLopHocPhan", b =>
                {
                    b.HasOne("QLDT_WPF.Models.LopHocPhan", "LopHocPhans")
                        .WithMany("ThoiGianLopHocPhans")
                        .HasForeignKey("IdLopHocPhan");

                    b.HasOne("QLDT_WPF.Models.ThoiGian", "ThoiGians")
                        .WithMany("ThoiGianLopHocPhans")
                        .HasForeignKey("IdThoiGian");

                    b.Navigation("LopHocPhans");

                    b.Navigation("ThoiGians");
                });

            modelBuilder.Entity("QLDT_WPF.Models.ChuongTrinhHoc", b =>
                {
                    b.Navigation("ChuongTrinhHocMonHocs");

                    b.Navigation("SinhViens");
                });

            modelBuilder.Entity("QLDT_WPF.Models.GiaoVien", b =>
                {
                    b.Navigation("LopHocPhans");
                });

            modelBuilder.Entity("QLDT_WPF.Models.Khoa", b =>
                {
                    b.Navigation("GiaoViens");

                    b.Navigation("MonHocs");

                    b.Navigation("SinhViens");
                });

            modelBuilder.Entity("QLDT_WPF.Models.LopHocPhan", b =>
                {
                    b.Navigation("Diems");

                    b.Navigation("SinhVienLopHocPhans");

                    b.Navigation("ThoiGianLopHocPhans");
                });

            modelBuilder.Entity("QLDT_WPF.Models.MonHoc", b =>
                {
                    b.Navigation("ChuongTrinhHocMonHocs");

                    b.Navigation("DangKyNguyenVongs");

                    b.Navigation("LopHocPhans");
                });

            modelBuilder.Entity("QLDT_WPF.Models.SinhVien", b =>
                {
                    b.Navigation("DangKyNguyenVongs");

                    b.Navigation("Diems");

                    b.Navigation("SinhVienLopHocPhans");
                });

            modelBuilder.Entity("QLDT_WPF.Models.ThoiGian", b =>
                {
                    b.Navigation("DangKyDoiLichs");

                    b.Navigation("ThoiGianLopHocPhans");
                });
#pragma warning restore 612, 618
        }
    }
}
