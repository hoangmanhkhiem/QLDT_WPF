using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
//
using QLDT_WPF.Data;
using QLDT_WPF.Models;
using QLDT_WPF.Dto;
using QLDT_WPF.ViewModels;
using QLDT_WPF.Services;
using System.Windows.Data;

namespace QLDT_WPF.Repositories
{
    public class IdentityRepository : IDisposable
    {
        private readonly QuanLySinhVienDbContext _context;
        private readonly IdentityDbContext _dbContext;
        private readonly SecurityService _securityService;

        public IdentityRepository()
        {
            // Handle connection Quan Ly Sinh Vien Db context
            var connectionString = ConfigurationManager
                .ConnectionStrings["QuanLySinhVienDbConnection"].ConnectionString;
            _context = new QuanLySinhVienDbContext(
                new DbContextOptionsBuilder<QuanLySinhVienDbContext>()
                    .UseSqlServer(connectionString)
                    .Options);

            // Handle connection Identity Db context
            var identityConnectionString = ConfigurationManager
                .ConnectionStrings["IdentityDbConnection"].ConnectionString;
            _dbContext = new IdentityDbContext(
                new DbContextOptionsBuilder<IdentityDbContext>()
                    .UseSqlServer(identityConnectionString)
                    .Options);

            // Init security service
            _securityService = new SecurityService();
        }

        // Dispose
        public void Dispose()
        {
            _context.Dispose();
            _dbContext.Dispose();
        }

        // Get all users
        public async Task<ApiResponse<List<UserDto>>> GetAll()
        {
            var list_users = await (
                from user in _dbContext.Users
                join userRole in _dbContext.UserRoles
                    on user.Id equals userRole.UserId
                join role in _dbContext.Roles
                    on userRole.RoleId equals role.Id
                select new UserDto
                {
                    Id = user.Id,
                    IdClaim = user.IdClaim,
                    UserName = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    FullName = user.FullName,
                    Address = user.Address,
                    IdRole = role.Id,
                    RoleName = role.Name,
                })
                .ToListAsync();

            return new ApiResponse<List<UserDto>>
            {
                Status = true,
                StatusCode = 200,
                Message = "Lấy danh sách người dùng thành công.",
                Data = list_users
            };
        }

        // Get all sinh vien
        public async Task<ApiResponse<List<SinhVienDto>>> GetAllSinhVien()
        {
            var list_sinhvien = await (
                from x in _context.SinhViens
                join y in _context.Khoas
                    on x.IdKhoa equals y.IdKhoa
                join z in _dbContext.Users
                    on x.IdSinhVien equals z.IdClaim
                select new SinhVienDto
                {
                    IdSinhVien = x.IdSinhVien,
                    IdKhoa = x.IdKhoa,
                    IdChuongTrinhHoc = x.IdChuongTrinhHoc,
                    HoTen = x.HoTen,
                    Lop = x.Lop,
                    NgaySinh = x.NgaySinh,
                    DiaChi = x.DiaChi,
                    TenKhoa = y.TenKhoa,
                    SoDienThoai = z.PhoneNumber,
                    Email = z.Email,
                })
                .ToListAsync();

            return new ApiResponse<List<SinhVienDto>>
            {
                Status = true,
                StatusCode = 200,
                Message = "Lấy danh sách sinh viên thành công.",
                Data = list_sinhvien
            };
        }

        // Get all giao vien
        public async Task<ApiResponse<List<GiaoVienDto>>> GetAllGiaoVien()
        {
            var list_gv = await (
                from gv in _context.GiaoViens
                join khoa in _context.Khoas
                    on gv.IdKhoa equals khoa.IdKhoa
                select new GiaoVienDto
                {
                    IdGiaoVien = gv.IdGiaoVien,
                    TenGiaoVien = gv.TenGiaoVien,
                    Email = gv.Email,
                    SoDienThoai = gv.SoDienThoai,
                    IdKhoa = gv.IdKhoa,
                    TenKhoa = khoa.TenKhoa,
                }
            ).ToListAsync();

            return new ApiResponse<List<GiaoVienDto>>
            {
                Status = true,
                StatusCode = 200,
                Message = "Lấy danh sách giáo viên thành công.",
                Data = list_gv
            };
        }

        // Create admin user
        public async Task<ApiResponse<UserDto>> CreateAdminUser(string username, string password)
        {
            // Add user
            var user = new UserCustom
            {
                UserName = username,
                PasswordHash = _securityService.Hash(password),
            };
            var result = await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Add role
            var adminRole = _dbContext.Roles.FirstOrDefault(x => x.Name.ToUpper() == "ADMIN");
            if (adminRole == null)
            {
                return new ApiResponse<UserDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy role ADMIN.",
                    Data = null
                };
            }

            var userRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = adminRole.Id,
            };

            await _dbContext.UserRoles.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<UserDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Tạo người dùng thành công.",
                Data = new UserDto
                {
                    UserName = username,
                }
            };
        }

        // Create sinh vien
        public async Task<ApiResponse<SinhVienDto>> CreateSinhVienUser(SinhVienDto sinhVien, string password)
        {
            // Add user
            var user = new UserCustom
            {
                UserName = sinhVien.IdSinhVien,
                IdClaim = sinhVien.IdSinhVien,
                Email = sinhVien.Email,
                PhoneNumber = sinhVien.SoDienThoai,
                FullName = sinhVien.HoTen,
                Address = sinhVien.DiaChi,
                PasswordHash = _securityService.Hash(password),
            };
            // Check if user, email, or phone number already exists
            var checkResult = await CheckExist(sinhVien.Email, sinhVien.Email, sinhVien.SoDienThoai);
            if (checkResult.status)
            {
                return new ApiResponse<SinhVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = checkResult.message,
                    Data = null
                };
            }
            var result = await _dbContext.Users.AddAsync(user);
            // Add role
            var sinhVienRole = _dbContext.Roles
                .FirstOrDefault(x => x.Name.ToUpper() == "SINHVIEN");
            var userRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = sinhVienRole.Id,
            };
            await _dbContext.UserRoles.AddAsync(userRole);
            // add sinh vien
            var sv = new SinhVien
            {
                IdSinhVien = sinhVien.IdSinhVien,
                IdKhoa = sinhVien.IdKhoa,
                IdChuongTrinhHoc = sinhVien.IdChuongTrinhHoc,
                HoTen = sinhVien.HoTen,
                Lop = sinhVien.Lop,
                NgaySinh = sinhVien.NgaySinh,
                DiaChi = sinhVien.DiaChi,
            };
            // Check khoa, chuong trinh hoc
            var khoa = await _context.Khoas.FirstOrDefaultAsync(x => x.IdKhoa == sinhVien.IdKhoa);
            if (khoa == null)
            {
                return new ApiResponse<SinhVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy khoa.",
                    Data = null
                };
            }
            var cth = await _context.ChuongTrinhHocs.FirstOrDefaultAsync(x => x.IdChuongTrinhHoc == sinhVien.IdChuongTrinhHoc);
            if (cth == null)
            {
                return new ApiResponse<SinhVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy chương trình học.",
                    Data = null
                };
            }
            var result_sv = await _context.SinhViens.AddAsync(sv);

            // Save changes
            await _dbContext.SaveChangesAsync();
            await _context.SaveChangesAsync();
            return new ApiResponse<SinhVienDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Tạo sinh viên thành công.",
                Data = sinhVien
            };
        }

        // Create list sinh vien form file
        public async Task<ApiResponse<List<SinhVienDto>>> CreateListSinhVienFromCSV(List<SinhVienDto> listSinhVien)
        {
            // Kiểm tra nếu danh sách null hoặc trống
            if (listSinhVien == null || !listSinhVien.Any())
            {
                return new ApiResponse<List<SinhVienDto>>
                {
                    Status = false,
                    Message = "File Không Được Để Trống!",
                    Data = null,
                };
            }

            List<SinhVienDto> listSinhVienError = new List<SinhVienDto>();
            HashSet<string> processedIds = new HashSet<string>();

            // Kiểm tra các bản ghi trùng lặp trong danh sách CSV
            foreach (var sv in listSinhVien)
            {
                if (processedIds.Contains(sv.IdSinhVien))
                {
                    sv.HoTen = $"Sinh Viên: {sv.HoTen} lỗi trùng ID {sv.IdSinhVien} trong file CSV";
                    listSinhVienError.Add(sv);
                    continue;
                }

                processedIds.Add(sv.IdSinhVien);
            }

            // Loại bỏ các bản ghi trùng lặp khỏi danh sách trước khi kiểm tra với CSDL
            var uniquelistSinhVien = listSinhVien.Except(listSinhVienError).ToList();

            // Kiểm tra với cơ sở dữ liệu và thêm vào danh sách lỗi nếu cần
            foreach (var sv in uniquelistSinhVien)
            {
                // Check id sinh vien trong co so du lieu
                var existingSinhVien = await _context.SinhViens
                    .FirstOrDefaultAsync(x => x.IdSinhVien == sv.IdSinhVien);
                if (existingSinhVien != null)
                {
                    sv.HoTen = $"Sinh Viên: {sv.HoTen} lỗi ID {sv.IdSinhVien} đã tồn tại trong CSDL";
                    listSinhVienError.Add(sv);
                    continue;
                }

                // Check mail, sdt, user name
                var checkResult = await CheckExist(sv.IdSinhVien, sv.Email, sv.SoDienThoai);
                if (checkResult.status == true)
                {
                    sv.HoTen = $"Sinh Viên: {sv.HoTen} lỗi {checkResult.message}";
                    listSinhVienError.Add(sv);
                    continue;
                } 

                // Check khoa, chuong trinh hoc
                var khoa = await _context.Khoas.FirstOrDefaultAsync(x => x.IdKhoa == sv.IdKhoa);
                if (khoa == null)
                {
                    sv.HoTen = $"Sinh Viên: {sv.HoTen} lỗi không tìm thấy khoa {sv.IdKhoa}";
                    listSinhVienError.Add(sv);
                    continue;
                }
                var cth = await _context.ChuongTrinhHocs.FirstOrDefaultAsync(x => x.IdChuongTrinhHoc == sv.IdChuongTrinhHoc);
                if (cth == null)
                {
                    sv.HoTen = $"Sinh Viên: {sv.HoTen} lỗi không tìm thấy chương trình học {sv.IdChuongTrinhHoc}";
                    listSinhVienError.Add(sv);
                    continue;
                }

                // Add Sinh Vien
                await _context.SinhViens.AddAsync(new SinhVien
                {
                    IdSinhVien = sv.IdSinhVien,
                    IdKhoa = sv.IdKhoa,
                    IdChuongTrinhHoc = sv.IdChuongTrinhHoc,
                    HoTen = sv.HoTen,
                    Lop = sv.Lop,
                    NgaySinh = sv.NgaySinh,
                    DiaChi = sv.DiaChi,
                });
                // Add User Sinh Vien
                await _dbContext.Users.AddAsync(new UserCustom
                {
                    UserName = sv.IdSinhVien,
                    IdClaim = sv.IdSinhVien,
                    Email = sv.Email,
                    PhoneNumber = sv.SoDienThoai,
                    FullName = sv.HoTen,
                    Address = sv.DiaChi,
                    PasswordHash = _securityService.Hash("123456789"),
                });
                // Add Role Sinh Vien
                var sinhVienRole = _dbContext.Roles
                    .FirstOrDefault(x => x.Name.ToUpper() == "SINHVIEN");
                var userRole = new IdentityUserRole<string>
                {
                    UserId = sv.IdSinhVien,
                    RoleId = sinhVienRole.Id,
                };
                await _dbContext.UserRoles.AddAsync(userRole);
            }

            // Nếu có bất kỳ lỗi nào trong quá trình xử lý
            if (listSinhVienError.Any())
            {
                return new ApiResponse<List<SinhVienDto>>
                {
                    Status = false,
                    Message = "Thêm Danh Sách Sinh Viên Thất Bại! Có lỗi trong danh sách Sinh Viên.",
                    Data = listSinhVienError,
                };
            }

            // Lưu thay đổi nếu mọi thứ thành công
            await _context.SaveChangesAsync();
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<List<SinhVienDto>>
            {
                Status = true,
                Message = "Thêm Danh Sách Sinh Viên Thành Công!",
                Data = listSinhVien,
            };
        }

        // create giao vien
        public async Task<ApiResponse<GiaoVienDto>> CreateGiaoVienUser(GiaoVienDto giaoVien, string password)
        {
            // Add user
            var user = new UserCustom
            {
                UserName = giaoVien.IdGiaoVien,
                IdClaim = giaoVien.IdGiaoVien,
                Email = giaoVien.Email,
                PhoneNumber = giaoVien.SoDienThoai,
                FullName = giaoVien.TenGiaoVien,
                PasswordHash = _securityService.Hash(password),
            };
            // Check if user, email, or phone number already exists
            var checkResult = await CheckExist(giaoVien.Email, giaoVien.Email, giaoVien.SoDienThoai);
            if (checkResult.status)
            {
                return new ApiResponse<GiaoVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = checkResult.message,
                    Data = null
                };
            }
            var result = await _dbContext.Users.AddAsync(user);
            // Add role
            var giaoVienRole = _dbContext.Roles
                .FirstOrDefault(x => x.Name.ToUpper() == "GIAOVIEN");
            var userRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = giaoVienRole.Id,
            };
            await _dbContext.UserRoles.AddAsync(userRole);
            // add giao vien
            var gv = new GiaoVien
            {
                IdGiaoVien = giaoVien.IdGiaoVien,
                TenGiaoVien = giaoVien.TenGiaoVien,
                Email = giaoVien.Email,
                SoDienThoai = giaoVien.SoDienThoai,
                IdKhoa = giaoVien.IdKhoa,
            };
            // Check khoa
            var khoa = await _context.Khoas.FirstOrDefaultAsync(x => x.IdKhoa == giaoVien.IdKhoa);
            if (khoa == null)
            {
                return new ApiResponse<GiaoVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy khoa.",
                    Data = null
                };
            }
            var result_gv = await _context.GiaoViens.AddAsync(gv);

            // Save changes
            await _dbContext.SaveChangesAsync();
            await _context.SaveChangesAsync();
            return new ApiResponse<GiaoVienDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Tạo giáo viên thành công.",
            };
        }

        // Create list giao vien form file
        public async Task<ApiResponse<List<GiaoVienDto>>> 
            CreateListGiaoVienFromCSV(List<GiaoVienDto> listGiaoVien)
        {
            // Kiểm tra nếu danh sách null hoặc trống
            if (listGiaoVien == null || !listGiaoVien.Any())
            {
                return new ApiResponse<List<GiaoVienDto>>
                {
                    Status = false,
                    Message = "File Không Được Để Trống!",
                    Data = null,
                };
            }

            List<GiaoVienDto> listGiaoVienError = new List<GiaoVienDto>();
            HashSet<string> processedIds = new HashSet<string>();

            // Kiểm tra các bản ghi trùng lặp trong danh sách CSV
            foreach (var gv in listGiaoVien)
            {
                if (processedIds.Contains(gv.IdGiaoVien))
                {
                    gv.TenGiaoVien = $"Giáo Viên: {gv.TenGiaoVien} lỗi trùng ID {gv.IdGiaoVien} trong file CSV";
                    listGiaoVienError.Add(gv);
                    continue;
                }
                processedIds.Add(gv.IdGiaoVien);
            }

            // Loại bỏ các bản ghi trùng lặp khỏi danh sách trước khi kiểm tra với CSDL
            var uniquelistGiaoVien = listGiaoVien.Except(listGiaoVienError).ToList();

            // Kiểm tra với cơ sở dữ liệu và thêm vào danh sách lỗi nếu cần
            foreach (var gv in uniquelistGiaoVien)
            {
                // Check id giao vien
                var existingGiaoVien = await _context.GiaoViens
                    .FirstOrDefaultAsync(x => x.IdGiaoVien == gv.IdGiaoVien);
                if (existingGiaoVien != null)
                {
                    gv.TenGiaoVien = $"Giáo Viên: {gv.TenGiaoVien} lỗi ID {gv.IdGiaoVien} đã tồn tại trong CSDL";
                    listGiaoVienError.Add(gv);
                    continue;
                }

                // Check email, phone number, user name
                var checkResult = await CheckExist(gv.IdGiaoVien, gv.Email, gv.SoDienThoai);
                if (checkResult.status == true)
                {
                    gv.TenGiaoVien = $"Giáo Viên: {gv.TenGiaoVien} lỗi {checkResult.message}";
                    listGiaoVienError.Add(gv);
                    continue;
                }

                // Check khoa
                var khoa = await _context.Khoas.FirstOrDefaultAsync(x => x.IdKhoa == gv.IdKhoa);
                if (khoa == null)
                {
                    gv.TenGiaoVien = $"Giáo Viên: {gv.TenGiaoVien} lỗi không tìm thấy khoa {gv.IdKhoa}";
                    listGiaoVienError.Add(gv);
                    continue;
                }

                // Add Giao Vien
                await _context.GiaoViens.AddAsync(new GiaoVien
                {
                    IdGiaoVien = gv.IdGiaoVien,
                    TenGiaoVien = gv.TenGiaoVien,
                    Email = gv.Email,
                    SoDienThoai = gv.SoDienThoai,
                    IdKhoa = gv.IdKhoa,
                });

                // Add User Giao Vien
                await _dbContext.Users.AddAsync(new UserCustom
                {
                    UserName = gv.IdGiaoVien,
                    IdClaim = gv.IdGiaoVien,
                    Email = gv.Email,
                    PhoneNumber = gv.SoDienThoai,
                    FullName = gv.TenGiaoVien,
                    Address = "",
                    PasswordHash = _securityService.Hash("123456789"),
                });

                // Add Role Giao Vien
                var giaoVienRole = _dbContext.Roles
                    .FirstOrDefault(x => x.Name.ToUpper() == "GIAOVIEN");
                var userRole = new IdentityUserRole<string>
                {
                    UserId = gv.IdGiaoVien,
                    RoleId = giaoVienRole.Id,
                };
                await _dbContext.UserRoles.AddAsync(userRole);
            }

            // Nếu có bất kỳ lỗi nào trong quá trình xử lý
            if (listGiaoVienError.Any())
            {
                return new ApiResponse<List<GiaoVienDto>>
                {
                    Status = false,
                    Message = "Thêm Danh Sách Sinh Giáo Thất Bại! Có lỗi trong danh sách Sinh Viên.",
                    Data = listGiaoVienError,
                };
            }

            // Lưu thay đổi nếu mọi thứ thành công
            await _context.SaveChangesAsync();
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<List<GiaoVienDto>>
            {
                Status = true,
                Message = "Thêm Danh Sách Giáo Viên Thành Công!",
                Data = listGiaoVien,
            };
        }

        // Handle login
        public async Task<(bool status, string message)> Login(string username, string password)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.UserName.ToUpper() == username.ToUpper());
            if (user == null)
            {
                return (false, "Tên đăng nhập không tồn tại.");
            }

            if (!_securityService.ValidateHash(password, user.PasswordHash))
            {
                return (false, "Mật khẩu không đúng.");
            }

            return (true, "");
        }

        // Handle get user information after login success
        public async Task<UserInformation> GetUserInformation(string userName)
        {
            var user_information = await (
                from user in _dbContext.Users
                where user.UserName.ToUpper() == userName.ToUpper()
                join userRole in _dbContext.UserRoles
                    on user.Id equals userRole.UserId
                join role in _dbContext.Roles
                    on userRole.RoleId equals role.Id
                orderby role.Name
                select new UserInformation
                {
                    IdUser = user.Id,
                    IdClaim = user.IdClaim,
                    UserName = user.UserName,
                    RoleName = role.Name,
                    FullName = user.FullName,
                    Image = user.ProfilePicture,
                }
            ).FirstOrDefaultAsync();

            return user_information;
        }

        // admin edit password sinh vien, giao vien
        public async Task<ApiResponse<UserDto>> AdminEditPassword(string id, string password)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.IdClaim == id);
            if (user == null)
            {
                return new ApiResponse<UserDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy người dùng.",
                    Data = null
                };
            }

            user.PasswordHash = _securityService.Hash(password);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<UserDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Đổi mật khẩu thành công.",
                Data = new UserDto
                {
                    UserName = user.UserName,
                }
            };
        }

        // user edit password
        public async Task<ApiResponse<UserDto>> UserEditPassword(string id, string oldPassword, string newPassword)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.IdClaim == id);
            if (user == null)
            {
                return new ApiResponse<UserDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy người dùng.",
                    Data = null
                };
            }

            if (!_securityService.ValidateHash(oldPassword, user.PasswordHash))
            {
                return new ApiResponse<UserDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Mật khẩu cũ không đúng.",
                    Data = null
                };
            }

            user.PasswordHash = _securityService.Hash(newPassword);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<UserDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Đổi mật khẩu thành công.",
                Data = new UserDto
                {
                    UserName = user.UserName,
                }
            };
        }

        // admin edit password admin
        public async Task<ApiResponse<UserDto>> AdminEditPasswordAdmin(string id, string password)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.IdClaim == id);
            if (user == null)
            {
                return new ApiResponse<UserDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy người dùng.",
                    Data = null
                };
            }

            user.PasswordHash = _securityService.Hash(password);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<UserDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Đổi mật khẩu thành công.",
                Data = new UserDto
                {
                    UserName = user.UserName,
                }
            };
        }

        // Upgrade information SinhVien
            public async Task<ApiResponse<SinhVienDto>> UpgradeSinhVien(SinhVienDto sinhVien)
            {
            // Find the existing sinh vien
            var existingSinhVien = await _context.SinhViens
                .FirstOrDefaultAsync(x => x.IdSinhVien == sinhVien.IdSinhVien);
            if (existingSinhVien == null)
            {
                return new ApiResponse<SinhVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy sinh viên.",
                    Data = null
                };
            }

            // Update the existing sinh vien
            existingSinhVien.HoTen = sinhVien.HoTen;
            existingSinhVien.DiaChi = sinhVien.DiaChi;

            // Upgrade user information
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.IdClaim == sinhVien.IdSinhVien);
            if (user == null)
            {
                return new ApiResponse<SinhVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy người dùng.",
                    Data = null
                };
            }

            // Check phone number
            var c_phone = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.PhoneNumber == sinhVien.SoDienThoai);
            if (c_phone != null && c_phone.IdClaim != sinhVien.IdSinhVien)
            {
                return new ApiResponse<SinhVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Số điện thoại đã tồn tại.",
                    Data = null
                };
            }

            // Check email 
            var c_email = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Email == sinhVien.Email);
            if (c_email != null && c_email.IdClaim != sinhVien.IdSinhVien)
            {
                return new ApiResponse<SinhVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Email đã tồn tại.",
                    Data = null
                };
            }

            user.Email = sinhVien.Email;
            user.PhoneNumber = sinhVien.SoDienThoai;
            user.FullName = sinhVien.HoTen;
            user.Address = sinhVien.DiaChi;

            _dbContext.Users.Update(user);

            await _context.SaveChangesAsync();
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<SinhVienDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Cập nhật sinh viên thành công.",
                Data = sinhVien
            };
        }

        // Upgrade information GiaoVien
        public async Task<ApiResponse<GiaoVienDto>> UpgradeGiaoVien(GiaoVienDto giaovien)
        {
            // Find the existing giao vien
            var existingGiaoVien = await _context.GiaoViens
                .FirstOrDefaultAsync(x => x.IdGiaoVien == giaovien.IdGiaoVien);
            if (existingGiaoVien == null)
            {
                return new ApiResponse<GiaoVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy giáo viên.",
                    Data = null
                };
            }

            // Update the existing giao vien
            existingGiaoVien.TenGiaoVien = giaovien.TenGiaoVien;
            existingGiaoVien.Email = giaovien.Email;
            existingGiaoVien.SoDienThoai = giaovien.SoDienThoai;
            existingGiaoVien.IdKhoa = giaovien.IdKhoa;
            // Save the changes
            await _context.SaveChangesAsync();

            // Upgrade user information
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.IdClaim == giaovien.IdGiaoVien);
            if (user == null)
            {
                return new ApiResponse<GiaoVienDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy người dùng.",
                    Data = null
                };
            }

            user.Email = giaovien.Email;
            user.PhoneNumber = giaovien.SoDienThoai;
            user.FullName = giaovien.TenGiaoVien;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<GiaoVienDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Cập nhật giáo viên thành công.",
                Data = giaovien
            };
        }

        // Helper check user name, email, phone number exist
        private async Task<(bool status, string message)> CheckExist(string username, string email, string phone)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.UserName == username);
            if (user != null)
            {
                return (true, "Tên đăng nhập đã tồn tại.");
            }

            user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                return (true, "Email đã tồn tại.");
            }

            user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            if (user != null)
            {
                return (true, "Số điện thoại đã tồn tại.");
            }

            return (false, "");
        }

        // Cập nhật thông tin người dùng
        public async Task<bool> UpdateUserAsync(UserDto user)
        {
            // Giả lập việc gửi dữ liệu lên server (thay bằng logic thực tế)
            await Task.Delay(1000); // Giả lập độ trễ
            return true; // Trả về true nếu cập nhật thành công
        }

        // Thay đổi mật khẩu
        public async Task<ApiResponse<UserDto>> ChangePassword(string id, string currentPassword, string newPassword)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.IdClaim == id);
            if (user == null)
            {
                return new ApiResponse<UserDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy người dùng.",
                    Data = null
                };
            }

            if (!_securityService.ValidateHash(currentPassword, user.PasswordHash))
            {
                return new ApiResponse<UserDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Mật khẩu cũ không đúng.",
                    Data = null
                };
            }

            user.PasswordHash = _securityService.Hash(newPassword);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return new ApiResponse<UserDto> {
                Status = true,
                StatusCode = 200,
                Message = "Đổi mật khẩu thành công.",
                Data = new UserDto
                {
                    UserName = user.UserName,
                }
            };
        }

        // Get Avatar User
        public async Task<ApiResponse<byte[]>> GetAvatar(string id)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.IdClaim == id);
            if (user == null){
                return new ApiResponse<byte[]>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy người dùng.",
                    Data = null
                };
            }

            return new ApiResponse<byte[]>
            {
                Status = true,
                StatusCode = 200,
                Message = "Lấy ảnh đại diện thành công.",
                Data = user.ProfilePicture
            };
        }

        // Set Avatar User
        public async Task<ApiResponse<UserDto>> SetAvatar(string id, byte[] image)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.IdClaim == id);
            if (user == null)
            {
                return new ApiResponse<UserDto>
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Không tìm thấy người dùng.",
                    Data = null
                };
            }

            user.ProfilePicture = image;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<UserDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Cập nhật ảnh đại diện thành công.",
                Data = new UserDto
                {
                    UserName = user.UserName,
                    ProfilePicture = user.ProfilePicture
                }
            };
        }
    }
}