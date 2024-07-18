namespace GHN1.Models
{

    public class Admin
    {
        public int AdminID { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string Quyen { get; set; } = "admin"; // Đặt giá trị mặc định
        public bool IsDeleted { get; set; } = false; // Đặt giá trị mặc định
    }
}
