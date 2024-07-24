using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using GHN1.Models;
using GHN1.Helpers;
using Microsoft.AspNetCore.Authorization;

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
    public IActionResult RegisterKhachHang([FromBody] RegisterModel registerModel)
    {
        if (string.IsNullOrEmpty(registerModel.Email) || string.IsNullOrEmpty(registerModel.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            // Kiểm tra xem email đã tồn tại hay chưa
            string checkQuery = "SELECT COUNT(*) FROM KhachHang WHERE Email = @Email";
            SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@Email", registerModel.Email);

            conn.Open();
            int count = (int)checkCmd.ExecuteScalar();
            conn.Close();

            if (count > 0)
            {
                return Conflict("Email đã tồn tại.");
            }

            // Mã hóa mật khẩu bằng MD5
            string hashedPassword = EncryptionHelper.ComputeMD5Hash(registerModel.MatKhau);

            // Nếu email chưa tồn tại, thêm khách hàng mới
            string query = "INSERT INTO KhachHang (HoTen, Email, MatKhau, Quyen, IsDeleted) VALUES (@HoTen, @Email, @MatKhau, @Quyen, @IsDeleted)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HoTen", registerModel.HoTen);
            cmd.Parameters.AddWithValue("@Email", registerModel.Email);
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);
            cmd.Parameters.AddWithValue("@Quyen", "khachhang"); // Giá trị mặc định
            cmd.Parameters.AddWithValue("@IsDeleted", false); // Giá trị mặc định

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        // Khởi tạo đối tượng KhachHang để trả về
        var khachHang = new KhachHang
        {
            HoTen = registerModel.HoTen,
            Email = registerModel.Email,
            MatKhau = registerModel.MatKhau,
            Quyen = "khachhang",
            IsDeleted = false
        };

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

        // Mã hóa mật khẩu trước khi so sánh
        string hashedPassword = EncryptionHelper.ComputeMD5Hash(model.MatKhau);

        KhachHang khachHang = null;
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "SELECT * FROM KhachHang WHERE Email = @Email AND MatKhau = @MatKhau AND IsDeleted = 0";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);

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
    [Authorize]
    // Cập nhật tài khoản khách hàng
    [HttpPut("update")]
    public IActionResult UpdateKhachHang([FromBody] KhachHang khachHang)
    {
        if (string.IsNullOrEmpty(khachHang.Email) || string.IsNullOrEmpty(khachHang.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        // Mã hóa mật khẩu trước khi lưu
        string hashedPassword = EncryptionHelper.ComputeMD5Hash(khachHang.MatKhau);

        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "UPDATE KhachHang SET HoTen = @HoTen, Email = @Email, MatKhau = @MatKhau WHERE KhachHangID = @KhachHangID AND IsDeleted = 0";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HoTen", khachHang.HoTen);
            cmd.Parameters.AddWithValue("@Email", khachHang.Email);
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);
            cmd.Parameters.AddWithValue("@KhachHangID", khachHang.KhachHangID);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            if (rowsAffected == 0)
                return NotFound(new { Message = "Khách hàng không tồn tại hoặc không thể cập nhật." });
        }

        return Ok(khachHang);
    }
    [Authorize]
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
                    var command = new SqlCommand("INSERT INTO DonHang (KhachHangID, TrangThaiID, DiaChiNhanHang, DiaChiGiaoHang, SoDienThoaiNguoiNhan, SoDienThoaiNguoiGui, NgayTao, NgayCapNhat, IsDeleted, KhoangCach) OUTPUT INSERTED.DonHangID VALUES (@KhachHangID, 1, @DiaChiNhanHang, @DiaChiGiaoHang, @SoDienThoaiNguoiNhan, @SoDienThoaiNguoiGui, GETDATE(), GETDATE(), 0, @KhoangCach)", connection, transaction);
                    command.Parameters.AddWithValue("@KhachHangID", donHang.KhachHangID);
                    command.Parameters.AddWithValue("@DiaChiNhanHang", donHang.DiaChiNhanHang);
                    command.Parameters.AddWithValue("@DiaChiGiaoHang", donHang.DiaChiGiaoHang);
                    command.Parameters.AddWithValue("@SoDienThoaiNguoiNhan", donHang.SoDienThoaiNguoiNhan);
                    command.Parameters.AddWithValue("@SoDienThoaiNguoiGui", donHang.SoDienThoaiNguoiGui);
                    command.Parameters.AddWithValue("@KhoangCach", donHang.KhoangCach); // Thêm tham số KhoangCach

                    var donHangId = (int)command.ExecuteScalar();

                    foreach (var chiTiet in donHang.ChiTietDonHang)
                    {
                        // Tính phí vận chuyển
                        var tienVanChuyen = TinhPhiVanChuyen(donHang.KhoangCach, chiTiet.KhoiLuong);

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
    private decimal TinhPhiVanChuyen(decimal khoangCach, decimal khoiLuong)
    {
        decimal donGiaKm = 5000m;
        decimal donGiaKhoiLuong = 1000m;

        decimal phiVanChuyen = (donGiaKm * khoangCach) + (donGiaKhoiLuong * khoiLuong);

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        if (currentTime >= new TimeSpan(18, 0, 0) || currentTime <= new TimeSpan(6, 0, 0))
        {
            phiVanChuyen += 2000m * khoangCach;
        }

        if (khoiLuong > 15)
        {
            phiVanChuyen += 1000m * khoangCach * (khoiLuong - 15);
        }

        if (khoangCach > 20)
        {
            phiVanChuyen += 1000m * (khoangCach - 20) / 2;
        }

        return phiVanChuyen;
    }
    [Authorize]
    [HttpGet("don-hang/{khachHangId}")]
    public IActionResult XemDanhSachDonHang(int khachHangId, [FromQuery] SearchQueryParams1 queryParams)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = @"
        SELECT 
            dh.DonHangID,
            dh.KhachHangID,
            dh.TrangThaiID,
            dh.NgayTao,
            dh.NgayCapNhat,
            dh.DiaChiNhanHang,
            dh.DiaChiGiaoHang,
            dh.SoDienThoaiNguoiNhan,
            dh.SoDienThoaiNguoiGui,
            tt.MoTaTrangThai,
            kh.HoTen AS HoTenKhachHang
        FROM 
            DonHang dh
        LEFT JOIN 
            TrangThaiDonHang tt ON dh.TrangThaiID = tt.TrangThaiID
        LEFT JOIN
            KhachHang kh ON dh.KhachHangID = kh.KhachHangID
        WHERE 
            dh.KhachHangID = @KhachHangID AND dh.IsDeleted = 0";

            if (queryParams.TrangThaiId.HasValue)
            {
                query += " AND dh.TrangThaiID = @TrangThaiID";
            }

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query += " AND (dh.DiaChiNhanHang LIKE @Search OR dh.DiaChiGiaoHang LIKE @Search OR dh.SoDienThoaiNguoiNhan LIKE @Search OR dh.SoDienThoaiNguoiGui LIKE @Search)";
            }

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@KhachHangID", khachHangId);

            if (queryParams.TrangThaiId.HasValue)
            {
                command.Parameters.AddWithValue("@TrangThaiID", queryParams.TrangThaiId.Value);
            }

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
                    NgayTao = reader["NgayTao"],
                    NgayCapNhat = reader["NgayCapNhat"],
                    DiaChiNhanHang = reader["DiaChiNhanHang"],
                    DiaChiGiaoHang = reader["DiaChiGiaoHang"],
                    SoDienThoaiNguoiNhan = reader["SoDienThoaiNguoiNhan"],
                    SoDienThoaiNguoiGui = reader["SoDienThoaiNguoiGui"],
                    MoTaTrangThai = reader["MoTaTrangThai"],
                    HoTenKhachHang = reader["HoTenKhachHang"]
                };
                donHangList.Add(donHang);
            }
            connection.Close();
            return Ok(donHangList);
        }
    }
    [Authorize]
    [HttpGet("xem-trang-thai-don-hang/{donHangId}")]
    public IActionResult XemTrangThaiDonHang(int donHangId, [FromQuery] SearchQueryParams queryParams)
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
                ldh.NgayTao AS NgayHoanHang,
                kh.HoTen AS HoTenKhachHang
            FROM 
                DonHang dh
            LEFT JOIN 
                TrangThaiDonHang tt ON dh.TrangThaiID = tt.TrangThaiID
            LEFT JOIN 
                LyDoHoanHang ldh ON dh.DonHangID = ldh.DonHangID
            LEFT JOIN 
                KhachHang kh ON dh.KhachHangID = kh.KhachHangID
            WHERE 
                dh.DonHangID = @DonHangID AND dh.IsDeleted = 0";

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query += " AND kh.HoTen LIKE @Search";
            }

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                command.Parameters.AddWithValue("@Search", "%" + queryParams.Search + "%");
            }

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
                    NgayHoanHang = reader["NgayHoanHang"] != DBNull.Value ? reader["NgayHoanHang"] : null,
                    HoTenKhachHang = reader["HoTenKhachHang"]
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
}
public class SearchQueryParams1
{
    public string Search { get; set; } = "";
    public int? TrangThaiId { get; set; }
}
