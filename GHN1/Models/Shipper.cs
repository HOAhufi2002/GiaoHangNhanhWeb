namespace GHN1.Models
{
    public class Shipper
    {
        public int ShipperID { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string Quyen { get; set; } = "shipper"; // Đặt giá trị mặc định
        public bool IsDeleted { get; set; } = false; // Đặt giá trị mặc định
    }


}
