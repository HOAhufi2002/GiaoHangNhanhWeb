using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GHN1.Models;
using GHN1.Helpers;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class ShipperController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ShipperController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection");
    }

    // Đăng ký Shipper
    [HttpPost("register")]
    public IActionResult RegisterShipper([FromBody] Shipper shipper)
    {
        if (string.IsNullOrEmpty(shipper.Email) || string.IsNullOrEmpty(shipper.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            // Kiểm tra xem email đã tồn tại hay chưa
            string checkQuery = "SELECT COUNT(*) FROM Shipper WHERE Email = @Email";
            SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@Email", shipper.Email);

            conn.Open();
            int count = (int)checkCmd.ExecuteScalar();
            conn.Close();

            if (count > 0)
            {
                return Conflict("Email đã tồn tại.");
            }

            // Mã hóa mật khẩu bằng MD5
            string hashedPassword = EncryptionHelper.ComputeMD5Hash(shipper.MatKhau);

            // Nếu email chưa tồn tại, thêm shipper mới
            shipper.Quyen = "shipper"; // Giá trị mặc định
            shipper.IsDeleted = false; // Giá trị mặc định

            string query = "INSERT INTO Shipper (HoTen, IsDeleted, MatKhau, Quyen, Email) VALUES (@HoTen, @IsDeleted, @MatKhau, @Quyen, @Email)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HoTen", shipper.HoTen);
            cmd.Parameters.AddWithValue("@IsDeleted", shipper.IsDeleted);
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);
            cmd.Parameters.AddWithValue("@Quyen", shipper.Quyen);
            cmd.Parameters.AddWithValue("@Email", shipper.Email);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        return Ok(shipper);
    }

    // Đăng nhập Shipper
    [HttpPost("login")]
    public IActionResult LoginShipper([FromBody] LoginModel model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        // Mã hóa mật khẩu trước khi so sánh
        string hashedPassword = EncryptionHelper.ComputeMD5Hash(model.MatKhau);

        Shipper shipper = null;
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "SELECT * FROM Shipper WHERE Email = @Email AND MatKhau = @MatKhau AND IsDeleted = 0";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                shipper = new Shipper
                {
                    ShipperID = (int)reader["ShipperID"],
                    HoTen = (string)reader["HoTen"],
                    IsDeleted = (bool)reader["IsDeleted"],
                    MatKhau = (string)reader["MatKhau"],
                    Quyen = (string)reader["Quyen"],
                    Email = (string)reader["Email"]
                };
            }
            conn.Close();
        }

        if (shipper == null)
            return Unauthorized();

        var token = GenerateJwtToken(shipper.Email, shipper.Quyen);
        return Ok(new { token, id = shipper.ShipperID, email = shipper.Email, quyen = shipper.Quyen, hoTen = shipper.HoTen });
    }
    [Authorize]
    // Cập nhật tài khoản shipper
    [HttpPut("update")]
    public IActionResult UpdateShipper([FromBody] Shipper shipper)
    {
        if (string.IsNullOrEmpty(shipper.Email) || string.IsNullOrEmpty(shipper.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        // Mã hóa mật khẩu trước khi lưu
        string hashedPassword = EncryptionHelper.ComputeMD5Hash(shipper.MatKhau);

        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "UPDATE Shipper SET HoTen = @HoTen, Email = @Email, MatKhau = @MatKhau WHERE ShipperID = @ShipperID AND IsDeleted = 0";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HoTen", shipper.HoTen);
            cmd.Parameters.AddWithValue("@Email", shipper.Email);
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);
            cmd.Parameters.AddWithValue("@ShipperID", shipper.ShipperID);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            if (rowsAffected == 0)
                return NotFound(new { Message = "Shipper không tồn tại hoặc không thể cập nhật." });
        }

        return Ok(shipper);
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
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Aud,_configuration["JWT:Audience"])
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    [Authorize]
    // Xóa Shipper
    [HttpDelete("{id}")]
    public IActionResult DeleteShipper(int id)
    {
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "UPDATE Shipper SET IsDeleted = 1 WHERE ShipperID = @ShipperID";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ShipperID", id);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            if (rowsAffected == 0)
                return NotFound();
        }

        return NoContent();
    }

    [Authorize]
    [HttpGet("don-hang-co-the-nhan")]
    public IActionResult XemDonHangCoTheNhan([FromQuery] SearchQueryParams queryParams)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = @"
            SELECT 
                dh.DonHangID,
                dh.KhachHangID,
                dh.TrangThaiID,
                tt.MoTaTrangThai,
                dh.DiaChiNhanHang,
                dh.DiaChiGiaoHang,
                dh.SoDienThoaiNguoiNhan,
                dh.SoDienThoaiNguoiGui,
                dh.NgayTao,
                dh.NgayCapNhat,
                kh.HoTen AS HoTenKhachHang
            FROM 
                DonHang dh
            LEFT JOIN 
                TrangThaiDonHang tt ON dh.TrangThaiID = tt.TrangThaiID
            LEFT JOIN
                KhachHang kh ON dh.KhachHangID = kh.KhachHangID
            WHERE 
                dh.TrangThaiID = 2 AND dh.IsDeleted = 0"; // Trạng thái "Đã Duyệt" hoặc trạng thái tương ứng

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query += " AND (kh.HoTen LIKE @Search OR dh.DiaChiNhanHang LIKE @Search OR dh.DiaChiGiaoHang LIKE @Search OR dh.SoDienThoaiNguoiNhan LIKE @Search OR dh.SoDienThoaiNguoiGui LIKE @Search)";
            }

            var command = new SqlCommand(query, connection);

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                command.Parameters.AddWithValue("@Search", "%" + queryParams.Search + "%");
            }

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
                    MoTaTrangThai = reader["MoTaTrangThai"],
                    DiaChiNhanHang = reader["DiaChiNhanHang"],
                    DiaChiGiaoHang = reader["DiaChiGiaoHang"],
                    SoDienThoaiNguoiNhan = reader["SoDienThoaiNguoiNhan"],
                    SoDienThoaiNguoiGui = reader["SoDienThoaiNguoiGui"],
                    NgayTao = reader["NgayTao"],
                    NgayCapNhat = reader["NgayCapNhat"],
                    HoTenKhachHang = reader["HoTenKhachHang"]
                };
                donHangList.Add(donHang);
            }
            connection.Close();
            return Ok(donHangList);
        }
    }
    [Authorize]
    // Cập nhật trạng thái đơn hàng sang Đang Giao Hàng và thêm ShipperID
    [HttpPost("nhan-don-hang")]
    public IActionResult NhanDonHang(int donHangId, int shipperId)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = "SELECT TrangThaiID FROM DonHang WHERE DonHangID = @DonHangID";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            var currentStatus = (int)command.ExecuteScalar();
            connection.Close();

            if (currentStatus != 2) // Trạng thái "Đã Duyệt"
            {
                return BadRequest(new { Message = "Đơn hàng không ở trạng thái 'Đã Duyệt'." });
            }

            command = new SqlCommand("UPDATE DonHang SET TrangThaiID = 3, ShipperID = @ShipperID, NgayCapNhat = GETDATE() WHERE DonHangID = @DonHangID", connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);
            command.Parameters.AddWithValue("@ShipperID", shipperId);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            if (rowsAffected == 0)
            {
                return NotFound(new { Message = "Đơn hàng không tồn tại hoặc không thể cập nhật." });
            }

            return Ok(new { Message = "Đơn hàng đã chuyển sang trạng thái 'Đang Giao Hàng'." });
        }
    }
    [Authorize]
    // Xác nhận đơn hàng đã giao
    [HttpPost("xac-nhan-giao-hang")]
    public IActionResult XacNhanGiaoHang(int donHangId)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = "SELECT TrangThaiID FROM DonHang WHERE DonHangID = @DonHangID";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            var currentStatus = (int)command.ExecuteScalar();
            connection.Close();

            if (currentStatus != 3) // Trạng thái "Đang Giao Hàng"
            {
                return BadRequest(new { Message = "Đơn hàng không ở trạng thái 'Đang Giao Hàng'." });
            }

            command = new SqlCommand("UPDATE DonHang SET TrangThaiID = 4, NgayCapNhat = GETDATE() WHERE DonHangID = @DonHangID", connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            if (rowsAffected == 0)
            {
                return NotFound(new { Message = "Đơn hàng không tồn tại hoặc không thể cập nhật." });
            }

            return Ok(new { Message = "Đơn hàng đã được xác nhận là 'Đã Giao'." });
        }
    }
    [Authorize]
    // Đánh dấu đơn hàng là Hoàn hàng đang xử lý
    [HttpPost("hoan-hang")]
    public IActionResult HoanHang(int donHangId, [FromBody] string lyDo)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = "SELECT TrangThaiID FROM DonHang WHERE DonHangID = @DonHangID";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            var currentStatus = (int)command.ExecuteScalar();
            connection.Close();

            if (currentStatus != 3) // Trạng thái "Đang Giao Hàng"
            {
                return BadRequest(new { Message = "Đơn hàng không ở trạng thái 'Đang Giao Hàng'." });
            }

            command = new SqlCommand("UPDATE DonHang SET TrangThaiID = 5, NgayCapNhat = GETDATE() WHERE DonHangID = @DonHangID", connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            if (rowsAffected == 0)
            {
                return NotFound(new { Message = "Đơn hàng không tồn tại hoặc không thể cập nhật." });
            }

            // Thêm lý do hoàn hàng vào bảng LyDoHoanHang
            command = new SqlCommand("INSERT INTO LyDoHoanHang (DonHangID, LyDo, NgayTao) VALUES (@DonHangID, @LyDo, GETDATE())", connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);
            command.Parameters.AddWithValue("@LyDo", lyDo);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return Ok(new { Message = "Đơn hàng đã chuyển sang trạng thái 'Hoàn hàng đang xử lý'." });
        }
    }
}
