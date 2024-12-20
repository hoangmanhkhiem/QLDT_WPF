﻿using QLDT_WPF.Dto;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.Grid;
using QLDT_WPF.Views.Shared;
using System.Windows.Media;
using QLDT_WPF.Repositories;
using Syncfusion.UI.Xaml.Scheduler;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using QLDT_WPF.Views.Shared.Components.Admin.Help;

namespace QLDT_WPF.Views.Components
{
    public partial class TeacherDetails : UserControl
    {
        public ContentControl TargetContentArea
        {
            get { return (ContentControl)GetValue(TargetContentAreaProperty); }
            set { SetValue(TargetContentAreaProperty, value); }
        }

        public static readonly DependencyProperty TargetContentAreaProperty =
            DependencyProperty.Register(nameof(TargetContentArea), typeof(ContentControl), typeof(TeacherDetails), new PropertyMetadata(null));

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;

            return FindParent<T>(parentObject);
        }

        // Variables
        private string idGiaoVien;
        private GiaoVienDto giaoVienDto;

        private GiaoVienRepository giaoVienRepository;
        private IdentityRepository identityRepository;
        private LopHocPhanRepository lopHocPhanRepository;
        private CalendarRepository calendarRepository;

        private ObservableCollection<LopHocPhanDto> lopHocPhan_collection;
        private ObservableCollection<ScheduleAppointment> lichGiaoVien_collection;

        private string constMH = "TeacherDetails";

        private string parent;

        // Constructor
        public TeacherDetails(string id, string parent)
        {
            InitializeComponent();

            idGiaoVien = id;

            giaoVienRepository = new GiaoVienRepository();
            identityRepository = new IdentityRepository();
            lopHocPhanRepository = new LopHocPhanRepository();
            calendarRepository = new CalendarRepository();

            lopHocPhan_collection = new ObservableCollection<LopHocPhanDto>();
            lichGiaoVien_collection = new ObservableCollection<ScheduleAppointment>();

            this.parent = parent;

            Loaded += async (s, e) =>
            {
                if (TargetContentArea == null)
                {
                    var parentWindow = FindParent<Window>(this); // Tìm parent window
                    if (parentWindow != null)
                    {
                        var contentArea = parentWindow.FindName("ContentArea") as ContentControl; // Tìm ContentArea
                        if (contentArea != null)
                        {
                            TargetContentArea = contentArea;
                        }
                        else
                        {
                            TargetContentArea = new ContentControl();
                        }
                    }
                    else
                    {
                        TargetContentArea = new ContentControl();
                    }
                }

                await InitAsync();
            };
            this.parent = parent;
        }

        private async Task InitAsync()
        {
            // Load thong tin giao vien
            await Load_Infomation_GiaoVien();

            // Load anh
            await Load_Anh();

            // Load Lop hoc phan
            await Load_LopHocPhan();

            // Load lich giao vien
            await Load_LichGiaoVien();

        }

        private async Task Load_Infomation_GiaoVien()
        {
            var req_gv = await giaoVienRepository.GetById(idGiaoVien);
            if (req_gv.Status == false)
            {
                MessageBox.Show(req_gv.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            giaoVienDto = req_gv.Data;

            // Hiển thị thông tin giáo viên
            txtFullName.Text = giaoVienDto.TenGiaoVien;
            txtEmail.Text = giaoVienDto.Email;
            txtPhoneNumber.Text = giaoVienDto.SoDienThoai;
            txtKhoa.Text = giaoVienDto.TenKhoa;

        }

        private async Task Load_Anh()
        {
            var req_avt = await identityRepository.GetAvatar(idGiaoVien);
                if (req_avt.Status == false || req_avt.Data == null || req_avt.Data.Length == 0)
                {
                    // Sử dụng ảnh mặc định từ tài nguyên ứng dụng
                    Uri defaultAvatarUri = new Uri("pack://application:,,,/Images/logoK.png"); // Thay đổi đường dẫn ảnh tùy ý
                    var defaultAvatarBitmap = new BitmapImage(defaultAvatarUri);

                    // Đặt ảnh mặc định cho ImageBrush
                    if (AvatarImageControl.Fill is ImageBrush defaultBrush)
                    {
                        defaultBrush.ImageSource = defaultAvatarBitmap;
                    }
                    else
                    {
                        AvatarImageControl.Fill = new ImageBrush(defaultAvatarBitmap) { Stretch = Stretch.UniformToFill };
                    }
                    return;
                }

            byte[] imageBytes = req_avt.Data;
            if (imageBytes == null || imageBytes.Length == 0)
            {
                MessageBox.Show("Dữ liệu ảnh không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Convert byte array to an image
            using (var stream = new System.IO.MemoryStream(imageBytes))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load image into memory
                bitmap.StreamSource = stream;
                bitmap.EndInit();

                // Assign the bitmap to the ImageBrush
                if (AvatarImageControl.Fill is ImageBrush imageBrush)
                {
                    imageBrush.ImageSource = bitmap;
                }
                else
                {
                    // If the Fill is not already an ImageBrush, create one
                    AvatarImageControl.Fill = new ImageBrush(bitmap) { Stretch = Stretch.UniformToFill };
                }
            }
        }

        private async Task Load_LopHocPhan()
        {
            var req_lhp = await lopHocPhanRepository.GetLopHocPhansFromGiaoVien(idGiaoVien);
            if (req_lhp.Status == false)
            {
                MessageBox.Show(req_lhp.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            lopHocPhan_collection.Clear();
            foreach (var lhp in req_lhp.Data)
            {
                // Đã Kết Thúc, Đang Diễn Ra, Sắp Diễn Ra - So Với Thời Gian Hiện Tại
                string StatusMessage = "";
                if (lhp.ThoiGianKetThuc < DateTime.Now)
                {
                    StatusMessage = "Đã Kết Thúc";
                }
                else if (lhp.ThoiGianBatDau < DateTime.Now && lhp.ThoiGianKetThuc > DateTime.Now)
                {
                    StatusMessage = "Đang Diễn Ra";
                }
                else if (lhp.ThoiGianBatDau > DateTime.Now)
                {
                    StatusMessage = "Sắp Diễn Ra";
                }

                lopHocPhan_collection.Add(new LopHocPhanDto
                {
                    IdLopHocPhan = lhp.IdLopHocPhan,
                    IdMonHoc = lhp.IdMonHoc,
                    IdGiaoVien = lhp.IdGiaoVien,

                    TenLopHocPhan = lhp.TenLopHocPhan,
                    TenGiaoVien = lhp.TenGiaoVien,
                    TenMonHoc = lhp.TenMonHoc,
                    ThoiGianBatDau = lhp.ThoiGianBatDau,
                    ThoiGianKetThuc = lhp.ThoiGianKetThuc,
                    StatusMessage = StatusMessage
                });
            }

            // Hiển thị danh sách lớp học phần
            DataGrid.ItemsSource = lopHocPhan_collection;
        }

        private async Task Load_LichGiaoVien()
        {
            var req_calendar = await calendarRepository.GetCalendarFromGiaoVien(idGiaoVien);
            if (req_calendar.Status == false)
            {
                MessageBox.Show(req_calendar.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            lichGiaoVien_collection.Clear();
            foreach (var it in req_calendar.Data)
            {
                lichGiaoVien_collection.Add(new ScheduleAppointment
                {
                    Subject = it.Title,
                    StartTime = it.Start ?? DateTime.MinValue,
                    EndTime = it.End ?? DateTime.MinValue,
                    Location = it.Location,
                    Notes = it.Description
                });
            }

            // Hiển thị lịch giáo viên
            calendar_giangvien.Appointments = lichGiaoVien_collection;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // redirect to edit teacher
            var teacherEditWindow = new TeacherEditWindow(giaoVienDto);
            if (TargetContentArea != null)
            {
                TargetContentArea.Content = teacherEditWindow;
            }
            else
            {
                MessageBox.Show("Không tìm thấy khu vực hiển thị nội dung!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // handle click text block ChiTietLopHocPhan_Click - redirect to detail lop hoc phan
        private void ChiTietLopHocPhan_Click(object sender, RoutedEventArgs e)
        {
            var idLopHocPhan = (sender as TextBlock)?.Tag.ToString();
            if (idLopHocPhan == null)
            {
                MessageBox.Show("Không tìm thấy ID của lớp học phần", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // redirect to detail lop hoc phan
            var lopHocPhanDetails = new LopHocPhanDetails(idLopHocPhan);
            if (TargetContentArea != null)
            {
                TargetContentArea.Content = lopHocPhanDetails;
            }
            else
            {
                MessageBox.Show("Không tìm thấy khu vực hiển thị nội dung!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // handle click text block ChiTietMonHoc_Click - redirect to detail mon hoc
        private void ChiTietMonHoc_Click(object sender, RoutedEventArgs e)
        {
            var idMonHoc = (sender as TextBlock)?.Tag.ToString();
            if (idMonHoc == null)
            {
                MessageBox.Show("Không tìm thấy ID của môn học", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // redirect to detail mon hoc
            var monHocDetails = new QLDT_WPF.Views.Shared.Components.Admin.View.SubjectDetails(idMonHoc,constMH);
            if (TargetContentArea != null)
            {
                TargetContentArea.Content = monHocDetails;
            }
            else
            {
                MessageBox.Show("Không tìm thấy khu vực hiển thị nội dung!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // private void DataGrid_CellTapped(object sender, GridCellTappedEventArgs e)
        // {
        //     // Kiểm tra nếu cột được click là "Giảng Viên"
        //     if (e.Record != null && e.Column.MappingName == "GiangVien")
        //     {
        //         // Lấy dữ liệu của hàng được chọn
        //         dynamic selectedRow = e.Record;

        //         // Hiển thị thông tin chi tiết trong các TextBox
        //         txtFullName.Text = selectedRow.GiangVien;
        //         txtEmail.Text = $"{selectedRow.GiangVien.ToLower().Replace(" ", "")}@school.edu.vn";
        //         txtPhoneNumber.Text = "0123456789"; // Dữ liệu giả lập
        //         txtAddress.Text = "Khoa Công Nghệ Thông Tin"; // Dữ liệu giả lập
        //     }
        // }
    }
}
