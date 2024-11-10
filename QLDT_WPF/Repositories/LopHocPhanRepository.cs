using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
//
using QLDT_WPF.Data;
using QLDT_WPF.Dto;
using Azure.Identity;
using QLDT_WPF.Models;

namespace QLDT_WPF.Repositories;

public class LopHocPhanRepository
{
    // Variables
    private readonly QuanLySinhVienDbContext _context;

    // Constructor
    public LopHocPhanRepository()
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
     * Lay tat ca lop hoc phan
     */
    public async Task<ApiResponse<List<LopHocPhanDto>>> GetAll()
    {
        var lhp = await (
            from l in _context.LopHocPhans
            join gv in _context.GiaoViens
                on l.IdGiaoVien equals gv.IdGiaoVien
            join mh in _context.MonHocs
                on l.IdMonHoc equals mh.IdMonHoc
            select new LopHocPhanDto
            {
                IdLopHocPhan = l.IdLopHocPhan,
                IdMonHoc = l.IdMonHoc,
                IdGiaoVien = l.IdGiaoVien,

                TenLopHocPhan = l.TenHocPhan,
                TenGiaoVien = gv.TenGiaoVien,
                TenMonHoc = mh.TenMonHoc,
                ThoiGianBatDau = l.ThoiGianBatDau,
                ThoiGianKetThuc = l.ThoiGianKetThuc,
            }
        ).ToListAsync();

        return new ApiResponse<List<LopHocPhanDto>>
        {
            Data = lhp,
            Status = true,
            Message = "Lấy dữ liệu thành công"
        };
    }

    /**
     * Lay lop hoc phan by id
     */
    public async Task<ApiResponse<List<LopHocPhanDto>>> GetById(string id)
    {
        var list_lhp = await (
            from lhp in _context.LopHocPhans
            where lhp.IdLopHocPhan == id
            join gv in _context.GiaoViens
                on lhp.IdGiaoVien equals gv.IdGiaoVien
            join mh in _context.MonHocs
                on lhp.IdMonHoc equals mh.IdMonHoc
            select new LopHocPhanDto
            {
                IdLopHocPhan = lhp.IdLopHocPhan,
                IdMonHoc = lhp.IdMonHoc,
                IdGiaoVien = lhp.IdGiaoVien,

                TenLopHocPhan = lhp.TenHocPhan,
                TenGiaoVien = gv.TenGiaoVien,
                TenMonHoc = mh.TenMonHoc,
                ThoiGianBatDau = lhp.ThoiGianBatDau,
                ThoiGianKetThuc = lhp.ThoiGianKetThuc,
            }
        ).ToListAsync();

        return new ApiResponse<List<LopHocPhanDto>>
        {
            Data = list_lhp,
            Status = true,
            Message = "Lấy dữ liệu thành công"
        };
    }

    /**
     * Sua thong tin lop hoc phan
     */
    public async Task<ApiResponse<LopHocPhanDto>> Edit(LopHocPhanDto lopHocPhan)
    {
        var lhp = await _context.LopHocPhans
            .FirstOrDefaultAsync(l => l.IdLopHocPhan == lopHocPhan.IdLopHocPhan);
        if (lhp == null)
        {
            return new ApiResponse<LopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy lớp học phần"
            };
        }

        // check giao vien, mon hoc ton tai
        var gv = await _context.GiaoViens
            .FirstOrDefaultAsync(g => g.IdGiaoVien == lopHocPhan.IdGiaoVien);
        if (gv == null)
        {
            return new ApiResponse<LopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy giáo viên"
            };
        }
        var mh = await _context.MonHocs
            .FirstOrDefaultAsync(m => m.IdMonHoc == lopHocPhan.IdMonHoc);
        if (mh == null)
        {
            return new ApiResponse<LopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy môn học"
            };
        }

        // check thoi gian thay doi
        if ((lhp.ThoiGianBatDau != lopHocPhan.ThoiGianBatDau
            || lhp.ThoiGianKetThuc != lopHocPhan.ThoiGianKetThuc)
            && lhp.ThoiGianBatDau >= DateTime.Now)
        {
            return new ApiResponse<LopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không thể thay đổi thời gian lớp học phần khi lớp học phần đã diễn ra"
            };
        }

        // update lop hoc phan
        lhp.TenHocPhan = lopHocPhan.TenLopHocPhan;
        lhp.IdGiaoVien = lopHocPhan.IdGiaoVien;
        lhp.IdMonHoc = lopHocPhan.IdMonHoc;
        lhp.ThoiGianBatDau = lopHocPhan.ThoiGianBatDau;
        lhp.ThoiGianKetThuc = lopHocPhan.ThoiGianKetThuc;

        await _context.SaveChangesAsync();

        return new ApiResponse<LopHocPhanDto>
        {
            Data = lopHocPhan,
            Status = true,
            Message = "Sửa thông tin lớp học phần thành công"
        };
    }

    /**
     * Them lop hoc phan
     */
    public async Task<ApiResponse<LopHocPhanDto>> Add(LopHocPhanDto lopHocPhan)
    {
        if (lopHocPhan.IdLopHocPhan == null)
        {
            lopHocPhan.IdLopHocPhan = Guid.NewGuid().ToString();
        }

        LopHocPhan newLopHocPhan = new LopHocPhan
        {
            IdLopHocPhan = lopHocPhan.IdLopHocPhan,
            IdMonHoc = lopHocPhan.IdMonHoc,
            IdGiaoVien = lopHocPhan.IdGiaoVien,
            TenHocPhan = lopHocPhan.TenLopHocPhan,
            ThoiGianBatDau = lopHocPhan.ThoiGianBatDau,
            ThoiGianKetThuc = lopHocPhan.ThoiGianKetThuc,
        };

        // check giao vien, mon hoc ton tai
        var gv = await _context.GiaoViens
            .FirstOrDefaultAsync(g => g.IdGiaoVien == lopHocPhan.IdGiaoVien);
        if (gv == null)
        {
            return new ApiResponse<LopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy giáo viên"
            };
        }
        var mh = await _context.MonHocs
            .FirstOrDefaultAsync(m => m.IdMonHoc == lopHocPhan.IdMonHoc);
        if (mh == null)
        {
            return new ApiResponse<LopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy môn học"
            };
        }

        // check thoi gian
        if (newLopHocPhan.ThoiGianBatDau <= DateTime.Now || newLopHocPhan.ThoiGianKetThuc <= DateTime.Now)
        {
            return new ApiResponse<LopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không thể thêm lớp học phần khi thời gian lớp học phần đã diễn ra"
            };
        }
        if (newLopHocPhan.ThoiGianBatDau >= newLopHocPhan.ThoiGianKetThuc)
        {
            return new ApiResponse<LopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Thời gian bắt đầu phải trước thời gian kết thúc"
            };
        }

        // handle add lop hoc phan
        await _context.LopHocPhans.AddAsync(newLopHocPhan);

        return
            new ApiResponse<LopHocPhanDto>
            {
                Data = lopHocPhan,
                Status = true,
                Message = "Thêm lớp học phần thành công"
            };
    }

    /**
     * Xoa lop hoc phan By Id 
     */
    public async Task<ApiResponse<LopHocPhanDto>> Delete(string id)
    {
        var lhp = _context.LopHocPhans
            .FirstOrDefault(l => l.IdLopHocPhan == id);
        if (lhp == null)
        {
            return new ApiResponse<LopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy lớp học phần"
            };
        }

        _context.Remove(lhp);
        await _context.SaveChangesAsync();

        return new ApiResponse<LopHocPhanDto>
        {
            Data = null,
            Status = true,
            Message = "Xóa lớp học phần thành công"
        };
    }

    /** 
     * Get lop hoc phan cua sinh vien tu id
     */
    public async Task<ApiResponse<List<LopHocPhanDto>>> GetLopHocPhansFromSinhVien(string id)
    {
        var sinhvien = await _context.SinhViens.
            FirstOrDefaultAsync(s => s.IdSinhVien == id);
        if (sinhvien == null)
        {
            return new ApiResponse<List<LopHocPhanDto>>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy sinh viên"
            };
        }

        var list_lhp = await (
            from sv_lhp in _context.SinhVienLopHocPhans
            where sv_lhp.IdSinhVien == id
            join lhp in _context.LopHocPhans
                on sv_lhp.IdLopHocPhan equals lhp.IdLopHocPhan
            join gv in _context.GiaoViens
                on lhp.IdGiaoVien equals gv.IdGiaoVien
            join mh in _context.MonHocs
                on lhp.IdMonHoc equals mh.IdMonHoc
            select new LopHocPhanDto
            {
                IdLopHocPhan = lhp.IdLopHocPhan,
                IdMonHoc = lhp.IdMonHoc,
                IdGiaoVien = lhp.IdGiaoVien,

                TenLopHocPhan = lhp.TenHocPhan,
                TenGiaoVien = gv.TenGiaoVien,
                TenMonHoc = mh.TenMonHoc,
                ThoiGianBatDau = lhp.ThoiGianBatDau,
                ThoiGianKetThuc = lhp.ThoiGianKetThuc,
            }
        ).ToListAsync();

        return new ApiResponse<List<LopHocPhanDto>>
        {
            Data = list_lhp,
            Status = true,
            Message = "Lấy dữ liệu thành công"
        };
    }

    /** 
     * Get lop hoc phan cua giao vien tu id
     */
    public async Task<ApiResponse<List<LopHocPhanDto>>>
        GetLopHocPhansFromGiaoVien(string id)
    {
        var giaovien = await _context.GiaoViens.
            FirstOrDefaultAsync(g => g.IdGiaoVien == id);
        if (giaovien == null)
        {
            return new ApiResponse<List<LopHocPhanDto>>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy giáo viên"
            };
        }

        var list_lhp = await (
            from lhp in _context.LopHocPhans
            where lhp.IdGiaoVien == id
            join gv in _context.GiaoViens
                on lhp.IdGiaoVien equals gv.IdGiaoVien
            join mh in _context.MonHocs
                on lhp.IdMonHoc equals mh.IdMonHoc
            select new LopHocPhanDto
            {
                IdLopHocPhan = lhp.IdLopHocPhan,
                IdMonHoc = lhp.IdMonHoc,
                IdGiaoVien = lhp.IdGiaoVien,

                TenLopHocPhan = lhp.TenHocPhan,
                TenGiaoVien = gv.TenGiaoVien,
                TenMonHoc = mh.TenMonHoc,
                ThoiGianBatDau = lhp.ThoiGianBatDau,
                ThoiGianKetThuc = lhp.ThoiGianKetThuc,
            }
        ).ToListAsync();

        return new ApiResponse<List<LopHocPhanDto>>
        {
            Data = list_lhp,
            Status = true,
            Message = "Lấy dữ liệu thành công"
        };
    }

    /**
     * Get lop hoc phan tu id mon hoc
     */
    public async Task<ApiResponse<List<LopHocPhanDto>>>
        GetLopHocPhansFromMonHoc(string id)
    {
        var monhoc = await _context.MonHocs
            .FirstOrDefaultAsync(x => x.IdMonHoc == id);
        if (monhoc == null)
        {
            return new ApiResponse<List<LopHocPhanDto>>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy môn học"
            };
        }

        var list_lhp = await (
            from lhp in _context.LopHocPhans
            where lhp.IdMonHoc == id
            join gv in _context.GiaoViens
                on lhp.IdGiaoVien equals gv.IdGiaoVien
            join mh in _context.MonHocs
                on lhp.IdMonHoc equals mh.IdMonHoc
            select new LopHocPhanDto
            {
                IdLopHocPhan = lhp.IdLopHocPhan,
                IdMonHoc = lhp.IdMonHoc,
                IdGiaoVien = lhp.IdGiaoVien,

                TenLopHocPhan = lhp.TenHocPhan,
                TenGiaoVien = gv.TenGiaoVien,
                TenMonHoc = mh.TenMonHoc,
                ThoiGianBatDau = lhp.ThoiGianBatDau,
                ThoiGianKetThuc = lhp.ThoiGianKetThuc,
            }
        ).ToListAsync();

        return new ApiResponse<List<LopHocPhanDto>>
        {
            Data = list_lhp,
            Status = true,
            Message = "Lấy dữ liệu thành công"
        };
    }

    /**
     * Thay doi thoi gian lop hoc phan 
     */
    public async Task<ApiResponse<ThayDoiThoiGianLopHocPhanDto>>
        ChangeTime(ThayDoiThoiGianLopHocPhanDto thayDoiThoiGianLopHocPhan)
    {
        var thoiGian = await _context.ThoiGians
            .FirstOrDefaultAsync(t => t.IdThoiGian == thayDoiThoiGianLopHocPhan.IdThoiGian);
        if (thoiGian == null)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy thời gian"
            };
        }

        var lopHocPhan = await _context.LopHocPhans
            .FirstOrDefaultAsync(l => l.IdLopHocPhan == thayDoiThoiGianLopHocPhan.IdLopHocPhan);
        if (lopHocPhan == null)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy lớp học phần"
            };
        }

        // Check thoi gian co thoa man khong
        if (thayDoiThoiGianLopHocPhan.ThoiGianBatDau >= thayDoiThoiGianLopHocPhan.ThoiGianKetThuc)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Thời gian bắt đầu phải trước thời gian kết thúc"
            };
        }

        // Check Thời Gian Truyền Vào Ở Quá Khứ
        if (thayDoiThoiGianLopHocPhan.ThoiGianBatDau <= DateTime.Now
            || thayDoiThoiGianLopHocPhan.ThoiGianKetThuc <= DateTime.Now)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không thể thay đổi thời gian lớp học phần khi thời gian lớp học phần đã diễn ra"
            };
        }
        
        // Check trong khoang cho phep
        if (thayDoiThoiGianLopHocPhan.ThoiGianBatDau < lopHocPhan.ThoiGianBatDau
            || thayDoiThoiGianLopHocPhan.ThoiGianKetThuc > lopHocPhan.ThoiGianKetThuc)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Thời gian thay đổi không nằm trong khoảng thời gian cho phép"
            };
        }

        // check thoi gian lop hoc phan co bi trung khong 
        var check_trung_thoi_gian_lop_hoc_phan = await (
            from tg in _context.ThoiGians
            join tg_lhp in _context.ThoiGianLopHocPhans
                on tg.IdThoiGian equals tg_lhp.IdThoiGian
            where tg_lhp.IdLopHocPhan == thayDoiThoiGianLopHocPhan.IdLopHocPhan
                && (
                    (thayDoiThoiGianLopHocPhan.ThoiGianBatDau >= tg.NgayBatDau
                        && thayDoiThoiGianLopHocPhan.ThoiGianBatDau <= tg.NgayKetThuc)
                    || (thayDoiThoiGianLopHocPhan.ThoiGianKetThuc >= tg.NgayBatDau
                        && thayDoiThoiGianLopHocPhan.ThoiGianKetThuc <= tg.NgayKetThuc)
                )
            select tg
        ).AnyAsync();
        if (check_trung_thoi_gian_lop_hoc_phan)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Thời gian lớp học phần bị trùng"
            };
        }

        // update thoi gian
        thoiGian.NgayBatDau = thayDoiThoiGianLopHocPhan.ThoiGianBatDau;
        thoiGian.NgayKetThuc = thayDoiThoiGianLopHocPhan.ThoiGianKetThuc;
        thoiGian.DiaDiem = thayDoiThoiGianLopHocPhan.DiaDiem;

        _context.ThoiGians.Update(thoiGian);
        await _context.SaveChangesAsync();

        return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
        {
            Data = thayDoiThoiGianLopHocPhan,
            Status = true,
            Message = "Thay đổi thời gian lớp học phần thành công"
        };
    }

    /**
     * Thêm thời gian cho lớp học phần
     */
    public async Task<ApiResponse<ThayDoiThoiGianLopHocPhanDto>>
        AddThoiGian(ThayDoiThoiGianLopHocPhanDto thoiGianLopHocPhan)
    {
        var mon = await (
            from lh in _context.LopHocPhans
            join mh in _context.MonHocs
                on lh.IdMonHoc equals mh.IdMonHoc
            where lh.IdLopHocPhan == thoiGianLopHocPhan.IdLopHocPhan
            select mh
        ).FirstOrDefaultAsync();

        if (mon == null)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không tìm thấy môn học"
            };
        }

        // Check da du tiet chua
        var tg_lhp = await (
            from tg in _context.ThoiGians
            join tglhp in _context.ThoiGianLopHocPhans
                on tg.IdThoiGian equals tglhp.IdThoiGian
            where tglhp.IdLopHocPhan == thoiGianLopHocPhan.IdLopHocPhan
            select tg
        ).ToListAsync();
        if (tg_lhp.Count()*3 >= mon.SoTietHoc){
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Đã đủ số tiết học không thể thêm thời gian"
            };
        }

        // Check thoi gian co thoa man khong
        if (thoiGianLopHocPhan.ThoiGianBatDau >= thoiGianLopHocPhan.ThoiGianKetThuc)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Thời gian bắt đầu phải trước thời gian kết thúc"
            };
        }

        // Check Thời Gian Truyền Vào Ở Quá Khứ
        if (thoiGianLopHocPhan.ThoiGianBatDau <= DateTime.Now
            || thoiGianLopHocPhan.ThoiGianKetThuc <= DateTime.Now)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Không thể thay đổi thời gian lớp học phần khi thời gian lớp học phần đã diễn ra"
            };
        }
        
        // Check trong khoang cho phep
        var lhp = await _context.LopHocPhans
            .FirstOrDefaultAsync(x => x.IdLopHocPhan == thoiGianLopHocPhan.IdLopHocPhan);
        if (thoiGianLopHocPhan.ThoiGianBatDau < lopHocPhan.ThoiGianBatDau
            || thoiGianLopHocPhan.ThoiGianKetThuc > lopHocPhan.ThoiGianKetThuc)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Thời gian thay đổi không nằm trong khoảng thời gian cho phép"
            };
        }

        // check thoi gian lop hoc phan co bi trung khong 
        var check_trung_thoi_gian_lop_hoc_phan = await (
            from tg in _context.ThoiGians
            join tg_lhp in _context.ThoiGianLopHocPhans
                on tg.IdThoiGian equals tg_lhp.IdThoiGian
            where tg_lhp.IdLopHocPhan == thoiGianLopHocPhan.IdLopHocPhan
                && (
                    (thoiGianLopHocPhan.ThoiGianBatDau >= tg.NgayBatDau
                        && thoiGianLopHocPhan.ThoiGianBatDau <= tg.NgayKetThuc)
                    || (thoiGianLopHocPhan.ThoiGianKetThuc >= tg.NgayBatDau
                        && thoiGianLopHocPhan.ThoiGianKetThuc <= tg.NgayKetThuc)
                )
            select tg
        ).AnyAsync();
        if (check_trung_thoi_gian_lop_hoc_phan)
        {
            return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
            {
                Data = null,
                Status = false,
                Message = "Thời gian lớp học phần bị trùng"
            };
        }

        // add thoi gian
        ThoiGian newThoiGian = new ThoiGian
        {
            IdThoiGian = Guid.NewGuid().ToString(),
            NgayBatDau = thoiGianLopHocPhan.ThoiGianBatDau,
            NgayKetThuc = thoiGianLopHocPhan.ThoiGianKetThuc,
            DiaDiem = thoiGianLopHocPhan.DiaDiem
        };

        await _context.ThoiGians.AddAsync(newThoiGian);
        await _context.SaveChangesAsync();

        ThoiGianLopHocPhan newThoiGianLopHocPhan = new ThoiGianLopHocPhan
        {
            IdThoiGian = newThoiGian.IdThoiGian,
            IdLopHocPhan = thoiGianLopHocPhan.IdLopHocPhan
        };

        await _context.ThoiGianLopHocPhans.AddAsync(newThoiGianLopHocPhan);
        await _context.SaveChangesAsync();

        return new ApiResponse<ThayDoiThoiGianLopHocPhanDto>
        {
            Data = thoiGianLopHocPhan,
            Status = true,
            Message = "Thêm thời gian lớp học phần thành công"
        };
    }
}