namespace GHN1.Models
{
    public class TaoDonHangModel
    {
        public int KhachHangID { get; set; }
        public string DiaChiNhanHang { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public string SoDienThoaiNguoiNhan { get; set; }
        public string SoDienThoaiNguoiGui { get; set; }
        public List<ChiTietDonHangModel> ChiTietDonHang { get; set; }
        public decimal KhoangCach { get; set; } // Thêm trường KhoangCach
    }
}
