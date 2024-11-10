using System.Collections.Generic;
using System.Linq;

//
using QLDT_WPF.Data;
using QLDT_WPF.Dto;

namespace QLDT_WPF.Repositories;

public class MonHocRepository
{
    // Variables
    private readonly QuanLySinhVienDbContext _context;

    // Constructor
    public MonHocRepository()
    {
        // Connect to database QuanLySinhVienDbContext
        var connectionString = ConfigurationManager.ConnectionStrings["QuanLySinhVienDbConnection"].ConnectionString;
        _context = new QuanLySinhVienDbContext(
            new DbContextOptionsBuilder<QuanLySinhVienDbContext>()
                .UseSqlServer(connectionString)
                .Options);
    }

    // Dispose
    public void Dispose()
    {
        _context.Dispose();
    }

    /**
     * Lay tat ca mon hoc
     */
    public async Task<ApiResponse<List<MonHocDto>>> GetAll()
    {
        var monhocs = await (
            from mh in _context.MonHocs
            join khoa in _context.Khoas on mh.IdKhoa equals khoa.IdKhoa
            select new MonHocDto {
                IdMonHoc = mh.IdMonHoc,
                IdKhoa = khoa.IdKhoa,

                TenMonHoc = mh.TenMonHoc,
                SoTinChi = mh.SoTinChi,
                SoTietHoc = mh.SoTietHoc,
                TenKhoa = khoa.TenKhoa,
            }
        ).ToListAsync();

        return new ApiResponse<List<MonHocDto>> {
            Data = monhocs,
            Success = true,
            Message = "Lấy dữ liệu thành công"
        };
    }

    /**
     * Lay mon hoc by id
     */

    /**
     * Lay mon hoc by id
     */


    /**
     * Sua thong tin mon hoc
     */


    /**
     * Them mon hoc
     */


    /**
     * Xoa mon hoc By Id 
     */

    /**
     * Get data mon hoc for giao vien with id giao vien
     */

}