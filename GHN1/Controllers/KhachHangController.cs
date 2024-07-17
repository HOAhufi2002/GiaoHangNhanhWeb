using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using GHN1.Models;

[Route("api/[controller]")]
[ApiController]
public class KhachHangController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public KhachHangController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection");
    }

    // Đăng ký Khách Hàng
    [HttpPost("register")]
    public IActionResult RegisterKhachHang([FromBody] KhachHang khachHang)
    {
        if (string.IsNullOrEmpty(khachHang.Email) || string.IsNullOrEmpty(khachHang.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        khachHang.Quyen = "khachhang";
        khachHang.IsDeleted = false;

        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "INSERT INTO KhachHang (HoTen, Email, MatKhau, Quyen, IsDeleted) VALUES (@HoTen, @Email, @MatKhau, @Quyen, @IsDeleted)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HoTen", khachHang.HoTen);
            cmd.Parameters.AddWithValue("@Email", khachHang.Email);
            cmd.Parameters.AddWithValue("@MatKhau", khachHang.MatKhau);
            cmd.Parameters.AddWithValue("@Quyen", khachHang.Quyen);
            cmd.Parameters.AddWithValue("@IsDeleted", khachHang.IsDeleted);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        return Ok(khachHang);
    }

    // Đăng nhập Khách Hàng
    [HttpPost("login")]
    public IActionResult LoginKhachHang([FromBody] LoginModel model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        KhachHang khachHang = null;
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "SELECT * FROM KhachHang WHERE Email = @Email AND MatKhau = @MatKhau AND IsDeleted = 0";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@MatKhau", model.MatKhau);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                khachHang = new KhachHang
                {
                    KhachHangID = (int)reader["KhachHangID"],
                    HoTen = (string)reader["HoTen"],
                    Email = (string)reader["Email"],
                    MatKhau = (string)reader["MatKhau"],
                    Quyen = (string)reader["Quyen"],
                    IsDeleted = (bool)reader["IsDeleted"]
                };
            }
            conn.Close();
        }

        if (khachHang == null)
            return Unauthorized();

        var token = GenerateJwtToken(khachHang.Email, khachHang.Quyen);
        return Ok(new { token, id = khachHang.KhachHangID, email = khachHang.Email, quyen = khachHang.Quyen, hoTen = khachHang.HoTen });

    }

    private string GenerateJwtToken(string email, string quyen)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim("quyen", quyen),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // Xóa Khách Hàng
    [HttpDelete("{id}")]
    public IActionResult DeleteKhachHang(int id)
    {
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "UPDATE KhachHang SET IsDeleted = 1 WHERE KhachHangID = @KhachHangID";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@KhachHangID", id);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            if (rowsAffected == 0)
                return NotFound();
        }

        return NoContent();
    }
    // Tạo đơn hàng mới
    [HttpPost("tao-don-hang")]
    public IActionResult TaoDonHang([FromBody] TaoDonHangModel donHang)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var command = new SqlCommand("INSERT INTO DonHang (KhachHangID, TrangThaiID, DiaChiNhanHang, DiaChiGiaoHang, SoDienThoaiNguoiNhan, SoDienThoaiNguoiGui, NgayTao, NgayCapNhat, IsDeleted) OUTPUT INSERTED.DonHangID VALUES (@KhachHangID, 1, @DiaChiNhanHang, @DiaChiGiaoHang, @SoDienThoaiNguoiNhan, @SoDienThoaiNguoiGui, GETDATE(), GETDATE(), 0)", connection, transaction);
                    command.Parameters.AddWithValue("@KhachHangID", donHang.KhachHangID);
                    command.Parameters.AddWithValue("@DiaChiNhanHang", donHang.DiaChiNhanHang);
                    command.Parameters.AddWithValue("@DiaChiGiaoHang", donHang.DiaChiGiaoHang);
                    command.Parameters.AddWithValue("@SoDienThoaiNguoiNhan", donHang.SoDienThoaiNguoiNhan);
                    command.Parameters.AddWithValue("@SoDienThoaiNguoiGui", donHang.SoDienThoaiNguoiGui);

                    var donHangId = (int)command.ExecuteScalar();

                    foreach (var chiTiet in donHang.ChiTietDonHang)
                    {
                        // Tính phí vận chuyển
                        var tienVanChuyen = 35 * chiTiet.KhoiLuong;

                        var chiTietCommand = new SqlCommand("INSERT INTO ChiTietDonHang (DonHangID, TenHangHoa, TienThuHoCOD, TienVanChuyen, KhoiLuong, IsDeleted) VALUES (@DonHangID, @TenHangHoa, @TienThuHoCOD, @TienVanChuyen, @KhoiLuong, 0)", connection, transaction);
                        chiTietCommand.Parameters.AddWithValue("@DonHangID", donHangId);
                        chiTietCommand.Parameters.AddWithValue("@TenHangHoa", chiTiet.TenHangHoa);
                        chiTietCommand.Parameters.AddWithValue("@TienThuHoCOD", chiTiet.TienThuHoCOD);
                        chiTietCommand.Parameters.AddWithValue("@TienVanChuyen", tienVanChuyen);
                        chiTietCommand.Parameters.AddWithValue("@KhoiLuong", chiTiet.KhoiLuong);

                        chiTietCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return Ok(new { Message = "Tạo đơn hàng thành công." });
                }
                catch
                {
                    transaction.Rollback();
                    return BadRequest(new { Message = "Tạo đơn hàng thất bại." });
                }
            }
        }
    }
    // Xem danh sách các đơn hàng của khách hàng
    [HttpGet("don-hang/{khachHangId}")]
    public IActionResult XemDanhSachDonHang(int khachHangId)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = @"
                SELECT 
                    DonHangID,
                    KhachHangID,
                    TrangThaiID,
                    NgayTao,
                    NgayCapNhat
                FROM 
                    DonHang
                WHERE 
                    KhachHangID = @KhachHangID";

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@KhachHangID", khachHangId);

            connection.Open();
            var reader = command.ExecuteReader();
            var donHangList = new List<object>();
            while (reader.Read())
            {
                var donHang = new
                {
                    DonHangID = reader["DonHangID"],
                    KhachHangID = reader["KhachHangID"],
                    TrangThaiID = reader["TrangThaiID"],
                    NgayTao = reader["NgayTao"],
                    NgayCapNhat = reader["NgayCapNhat"]
                };
                donHangList.Add(donHang);
            }
            connection.Close();
            return Ok(donHangList);
        }
    }

    // Xem trạng thái đơn hàng
    [HttpGet("xem-trang-thai-don-hang/{donHangId}")]
    public IActionResult XemTrangThaiDonHang(int donHangId)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = @"
                SELECT 
                    dh.DonHangID,
                    dh.KhachHangID,
                    dh.ShipperID,
                    dh.TrangThaiID,
                    tt.MoTaTrangThai,
                    dh.DiaChiNhanHang,
                    dh.DiaChiGiaoHang,
                    dh.SoDienThoaiNguoiNhan,
                    dh.SoDienThoaiNguoiGui,
                    dh.NgayTao,
                    dh.NgayCapNhat,
                    dh.IsDeleted,
                    ldh.LyDo,
                    ldh.NgayTao AS NgayHoanHang
                FROM 
                    DonHang dh
                LEFT JOIN 
                    TrangThaiDonHang tt ON dh.TrangThaiID = tt.TrangThaiID
                LEFT JOIN 
                    LyDoHoanHang ldh ON dh.DonHangID = ldh.DonHangID
                WHERE 
                    dh.DonHangID = @DonHangID";

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var donHang = new
                {
                    DonHangID = reader["DonHangID"],
                    KhachHangID = reader["KhachHangID"],
                    ShipperID = reader["ShipperID"],
                    TrangThaiID = reader["TrangThaiID"],
                    MoTaTrangThai = reader["MoTaTrangThai"],
                    DiaChiNhanHang = reader["DiaChiNhanHang"],
                    DiaChiGiaoHang = reader["DiaChiGiaoHang"],
                    SoDienThoaiNguoiNhan = reader["SoDienThoaiNguoiNhan"],
                    SoDienThoaiNguoiGui = reader["SoDienThoaiNguoiGui"],
                    NgayTao = reader["NgayTao"],
                    NgayCapNhat = reader["NgayCapNhat"],
                    IsDeleted = reader["IsDeleted"],
                    LyDo = reader["LyDo"] != DBNull.Value ? reader["LyDo"] : null,
                    NgayHoanHang = reader["NgayHoanHang"] != DBNull.Value ? reader["NgayHoanHang"] : null
                };
                connection.Close();
                return Ok(donHang);
            }
            else
            {
                connection.Close();
                return NotFound(new { Message = "Đơn hàng không tồn tại." });
            }
        }
    }
}
