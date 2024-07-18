namespace GHN1.Models
{
    public class KhachHang
    {
        public int KhachHangID { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string Quyen { get; set; } = "khachhang"; 
        public bool IsDeleted { get; set; } = false; 
    }

}
