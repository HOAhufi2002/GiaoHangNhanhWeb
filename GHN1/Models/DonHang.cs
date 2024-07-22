namespace GHN1.Models
{
    public class DonHang
    {
        public int DonHangID { get; set; }
        public int KhachHangID { get; set; }
        public int? ShipperID { get; set; }
        public int TrangThaiID { get; set; }
        public string TenTrangThai { get; set; } // Thêm trường này

        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public bool IsDeleted { get; set; }
        public string DiaChiNhanHang { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public string SoDienThoaiNguoiNhan { get; set; }
        public string SoDienThoaiNguoiGui { get; set; }
        public int? AdminID { get; set; } // Thêm trường này
        public string HoTenKhachHang { get; set; } // Thêm thuộc tính này

    }

}
