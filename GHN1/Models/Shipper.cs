namespace GHN1.Models
{
    public class Shipper
    {
        public int ShipperID { get; set; }
        public string HoTen { get; set; }
        public bool IsDeleted { get; set; }
        public string MatKhau { get; set; }
        public string Quyen { get; set; }
        public string Email { get; set; } // Thêm thuộc tính Email
    }


}
