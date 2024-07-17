namespace GHN1.Models
{
    public class Admin
    {
        public int AdminID { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string Quyen { get; set; }
        public bool IsDeleted { get; set; }
    }

}
