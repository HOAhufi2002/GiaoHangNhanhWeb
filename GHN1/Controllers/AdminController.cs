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
public class AdminController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AdminController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection");
    }
    [Authorize]
    [HttpGet("hoan-hang-dang-xu-ly")]
    public IActionResult GetHoanHangDangXuLy([FromQuery] HoanHangDangXuLyQueryParams queryParams)
    {
        List<DonHang> donHangs = new List<DonHang>();
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = @"
            SELECT 
                d.DonHangID,
                d.KhachHangID,
                d.ShipperID,
                d.TrangThaiID,
                t.MoTaTrangThai AS TenTrangThai, 
                d.NgayTao, 
                d.NgayCapNhat, 
                d.IsDeleted, 
                d.DiaChiNhanHang, 
                d.DiaChiGiaoHang, 
                d.SoDienThoaiNguoiNhan, 
                d.SoDienThoaiNguoiGui, 
                k.HoTen AS HoTenKhachHang
            FROM 
                DonHang d
            JOIN 
                TrangThaiDonHang t ON d.TrangThaiID = t.TrangThaiID
            JOIN 
                KhachHang k ON d.KhachHangID = k.KhachHangID
            WHERE 
                d.TrangThaiID = 5 AND d.IsDeleted = 0"; // Giả sử 5 là ID cho trạng thái "Hoàn hàng đang xử lý"

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query += " AND (k.HoTen LIKE @Search OR d.DiaChiNhanHang LIKE @Search OR d.DiaChiGiaoHang LIKE @Search OR d.SoDienThoaiNguoiNhan LIKE @Search OR d.SoDienThoaiNguoiGui LIKE @Search)";
            }

            SqlCommand cmd = new SqlCommand(query, conn);

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                cmd.Parameters.AddWithValue("@Search", "%" + queryParams.Search + "%");
            }

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DonHang donHang = new DonHang
                {
                    DonHangID = (int)reader["DonHangID"],
                    KhachHangID = (int)reader["KhachHangID"],
                    ShipperID = reader["ShipperID"] as int?,
                    TrangThaiID = (int)reader["TrangThaiID"],
                    TenTrangThai = reader["TenTrangThai"].ToString(),
                    NgayTao = reader["NgayTao"] as DateTime?,
                    NgayCapNhat = reader["NgayCapNhat"] as DateTime?,
                    IsDeleted = (bool)reader["IsDeleted"],
                    DiaChiNhanHang = reader["DiaChiNhanHang"].ToString(),
                    DiaChiGiaoHang = reader["DiaChiGiaoHang"].ToString(),
                    SoDienThoaiNguoiNhan = reader["SoDienThoaiNguoiNhan"].ToString(),
                    SoDienThoaiNguoiGui = reader["SoDienThoaiNguoiGui"].ToString(),
                    HoTenKhachHang = reader["HoTenKhachHang"].ToString()
                };
                donHangs.Add(donHang);
            }
            conn.Close();
        }
        return Ok(donHangs);
    }


    [HttpGet("cho-duyet")]
    public IActionResult GetDonHangChoDuyet([FromQuery] DonHangChoDuyetQueryParams queryParams)
    {
        List<DonHang> donHangs = new List<DonHang>();
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = @"SELECT d.DonHangID, d.KhachHangID, d.ShipperID, d.TrangThaiID, t.MoTaTrangThai AS TenTrangThai, 
                         d.NgayTao, d.NgayCapNhat, d.IsDeleted, d.DiaChiNhanHang, d.DiaChiGiaoHang, 
                         d.SoDienThoaiNguoiNhan, d.SoDienThoaiNguoiGui, k.HoTen AS HoTenKhachHang
                         FROM DonHang d
                         JOIN TrangThaiDonHang t ON d.TrangThaiID = t.TrangThaiID
                         JOIN KhachHang k ON d.KhachHangID = k.KhachHangID
                         WHERE d.TrangThaiID = 1 AND d.IsDeleted = 0";

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query += " AND (k.HoTen LIKE @Search OR d.DiaChiNhanHang LIKE @Search OR d.DiaChiGiaoHang LIKE @Search OR d.SoDienThoaiNguoiNhan LIKE @Search OR d.SoDienThoaiNguoiGui LIKE @Search)";
            }

            SqlCommand cmd = new SqlCommand(query, conn);

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                cmd.Parameters.AddWithValue("@Search", "%" + queryParams.Search + "%");
            }

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DonHang donHang = new DonHang
                {
                    DonHangID = (int)reader["DonHangID"],
                    KhachHangID = (int)reader["KhachHangID"],
                    ShipperID = reader["ShipperID"] as int?,
                    TrangThaiID = (int)reader["TrangThaiID"],
                    TenTrangThai = reader["TenTrangThai"].ToString(),
                    NgayTao = reader["NgayTao"] as DateTime?,
                    NgayCapNhat = reader["NgayCapNhat"] as DateTime?,
                    IsDeleted = (bool)reader["IsDeleted"],
                    DiaChiNhanHang = reader["DiaChiNhanHang"].ToString(),
                    DiaChiGiaoHang = reader["DiaChiGiaoHang"].ToString(),
                    SoDienThoaiNguoiNhan = reader["SoDienThoaiNguoiNhan"].ToString(),
                    SoDienThoaiNguoiGui = reader["SoDienThoaiNguoiGui"].ToString(),
                    HoTenKhachHang = reader["HoTenKhachHang"].ToString() // Lấy thêm thông tin HoTen
                };
                donHangs.Add(donHang);
            }
            conn.Close();
        }
        return Ok(donHangs);
    }

    [Authorize]
    [HttpPost("register")]
    public IActionResult RegisterAdmin([FromBody] Admin admin)
    {
        if (string.IsNullOrEmpty(admin.Email) || string.IsNullOrEmpty(admin.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            // Kiểm tra xem email đã tồn tại hay chưa
            string checkQuery = "SELECT COUNT(*) FROM Admin WHERE Email = @Email";
            SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@Email", admin.Email);

            conn.Open();
            int count = (int)checkCmd.ExecuteScalar();
            conn.Close();

            if (count > 0)
            {
                return Conflict("Email đã tồn tại.");
            }

            // Mã hóa mật khẩu bằng MD5
            string hashedPassword = EncryptionHelper.ComputeMD5Hash(admin.MatKhau);

            // Nếu email chưa tồn tại, thêm admin mới
            admin.Quyen = "admin"; // Giá trị mặc định
            admin.IsDeleted = false; // Giá trị mặc định

            string query = "INSERT INTO Admin (HoTen, Email, MatKhau, Quyen, IsDeleted) VALUES (@HoTen, @Email, @MatKhau, @Quyen, @IsDeleted)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HoTen", admin.HoTen);
            cmd.Parameters.AddWithValue("@Email", admin.Email);
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);
            cmd.Parameters.AddWithValue("@Quyen", admin.Quyen);
            cmd.Parameters.AddWithValue("@IsDeleted", admin.IsDeleted);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        return Ok(admin);
    }

    // Đăng nhập Admin
    [HttpPost("login")]
    public IActionResult LoginAdmin([FromBody] LoginModel model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        // Mã hóa mật khẩu trước khi so sánh
        string hashedPassword = EncryptionHelper.ComputeMD5Hash(model.MatKhau);

        Admin admin = null;
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "SELECT * FROM Admin WHERE Email = @Email AND MatKhau = @MatKhau AND IsDeleted = 0";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                admin = new Admin
                {
                    AdminID = (int)reader["AdminID"],
                    HoTen = (string)reader["HoTen"],
                    Email = (string)reader["Email"],
                    MatKhau = (string)reader["MatKhau"],
                    Quyen = (string)reader["Quyen"],
                    IsDeleted = (bool)reader["IsDeleted"]
                };
            }
            conn.Close();
        }

        if (admin == null)
            return Unauthorized();

        var token = GenerateJwtToken(admin.Email, admin.Quyen);
        return Ok(new { token, id = admin.AdminID, email = admin.Email, quyen = admin.Quyen, hoTen = admin.HoTen });
    }
    
    [Authorize]
    // Cập nhật tài khoản admin
    [HttpPut("update")]
    public IActionResult UpdateAdmin([FromBody] Admin admin)
    {
        if (string.IsNullOrEmpty(admin.Email) || string.IsNullOrEmpty(admin.MatKhau))
        {
            return BadRequest("Email và Mật khẩu không được để trống.");
        }

        // Mã hóa mật khẩu trước khi lưu
        string hashedPassword = EncryptionHelper.ComputeMD5Hash(admin.MatKhau);

        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "UPDATE Admin SET HoTen = @HoTen, Email = @Email, MatKhau = @MatKhau WHERE AdminID = @AdminID AND IsDeleted = 0";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@HoTen", admin.HoTen);
            cmd.Parameters.AddWithValue("@Email", admin.Email);
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);
            cmd.Parameters.AddWithValue("@AdminID", admin.AdminID);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            if (rowsAffected == 0)
                return NotFound(new { Message = "Admin không tồn tại hoặc không thể cập nhật." });
        }

        return Ok(admin);
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
    // Xóa Admin
    [HttpDelete("{id}")]
    public IActionResult DeleteAdmin(int id)
    {
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            string query = "UPDATE Admin SET IsDeleted = 1 WHERE AdminID = @AdminID";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@AdminID", id);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            if (rowsAffected == 0)
                return NotFound();
        }

        return NoContent();
    }

    [HttpPost("duyet-don-hang")]
    public IActionResult DuyetDonHang(int donHangId, int adminId)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            // Kiểm tra trạng thái hiện tại của đơn hàng
            var query = "SELECT TrangThaiID FROM DonHang WHERE DonHangID = @DonHangID";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            var currentStatus = (int)command.ExecuteScalar();
            connection.Close();

            if (currentStatus != 1) // Trạng thái "Chờ Duyệt"
            {
                return BadRequest(new { Message = "Đơn hàng không ở trạng thái 'Chờ Duyệt'." });
            }

            // Cập nhật trạng thái đơn hàng và AdminID
            query = "UPDATE DonHang SET TrangThaiID = 2, NgayCapNhat = GETDATE(), AdminID = @AdminID WHERE DonHangID = @DonHangID";
            command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);
            command.Parameters.AddWithValue("@AdminID", adminId);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            if (rowsAffected == 0)
            {
                return NotFound(new { Message = "Đơn hàng không tồn tại hoặc không thể cập nhật." });
            }

            return Ok(new { Message = "Đơn hàng đã được duyệt." });
        }
    }
    // Xác nhận hoàn hàng

    [Authorize]
    [HttpPost("xac-nhan-hoan-hang")]
    public IActionResult XacNhanHoanHang(int donHangId, int adminId)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = "SELECT TrangThaiID FROM DonHang WHERE DonHangID = @DonHangID";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            var currentStatus = (int)command.ExecuteScalar();
            connection.Close();

            if (currentStatus != 5) // Trạng thái "Hoàn hàng đang xử lý"
            {
                return BadRequest(new { Message = "Đơn hàng không ở trạng thái 'Hoàn hàng đang xử lý'." });
            }

            command = new SqlCommand("UPDATE DonHang SET TrangThaiID = 6, NgayCapNhat = GETDATE() WHERE DonHangID = @DonHangID", connection);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            if (rowsAffected == 0)
            {
                return NotFound(new { Message = "Đơn hàng không tồn tại hoặc không thể cập nhật." });
            }

            // Cập nhật AdminID vào bảng LyDoHoanHang
            command = new SqlCommand("UPDATE LyDoHoanHang SET AdminID = @AdminID WHERE DonHangID = @DonHangID", connection);
            command.Parameters.AddWithValue("@AdminID", adminId);
            command.Parameters.AddWithValue("@DonHangID", donHangId);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return Ok(new { Message = "Đơn hàng đã được xác nhận hoàn hàng." });
        }
    }

    [Authorize]
    // Thống kê số lượng đơn hàng đã hoàn tất giao
    [HttpGet("thong-ke-don-hang-hoan-tat-giao")]
    [Authorize]
    public IActionResult ThongKeDonHangHoanTatGiao()
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = @"
                SELECT 
                    COUNT(*) AS SoLuongDonHangHoanTatGiao
                FROM 
                    DonHang
                WHERE 
                    TrangThaiID = 4"; // Trạng thái "Đã Giao"

            var command = new SqlCommand(query, connection);

            connection.Open();
            var soLuong = (int)command.ExecuteScalar();
            connection.Close();

            return Ok(new { SoLuongDonHangHoanTatGiao = soLuong });
        }
    }
    [Authorize]
    // Thống kê số lượng đơn hàng đã hoàn hàng
    [HttpGet("thong-ke-don-hang-hoan-hang")]
    public IActionResult ThongKeDonHangHoanHang()
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = @"
                SELECT 
                    COUNT(*) AS SoLuongDonHangHoanHang
                FROM 
                    DonHang
                WHERE 
                    TrangThaiID = 6"; // Trạng thái "Hoàn hàng đã hoàn tất"

            var command = new SqlCommand(query, connection);

            connection.Open();
            var soLuong = (int)command.ExecuteScalar();
            connection.Close();

            return Ok(new { SoLuongDonHangHoanHang = soLuong });
        }
    }
    [Authorize]
    [HttpGet("danh-sach-khach-hang")]
    public IActionResult DanhSachKhachHang([FromQuery] SearchQueryParams queryParams)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = @"
            SELECT 
                KhachHangID,
                HoTen,
                Email,
                IsDeleted
            FROM 
                KhachHang
            WHERE 
                IsDeleted = 0";

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query += " AND (HoTen LIKE @Search OR Email LIKE @Search)";
            }

            var command = new SqlCommand(query, connection);

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                command.Parameters.AddWithValue("@Search", "%" + queryParams.Search + "%");
            }

            connection.Open();
            var reader = command.ExecuteReader();
            var khachHangList = new List<object>();
            while (reader.Read())
            {
                var khachHang = new
                {
                    KhachHangID = reader["KhachHangID"],
                    HoTen = reader["HoTen"],
                    Email = reader["Email"],
                    IsDeleted = reader["IsDeleted"]
                };
                khachHangList.Add(khachHang);
            }
            connection.Close();
            return Ok(khachHangList);
        }
    }
    [Authorize]
    [HttpGet("danh-sach-nguoi-dung")]
    public IActionResult DanhSachNguoiDung([FromQuery] SearchQueryParams queryParams)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var queryKhachHang = @"
            SELECT 
                KhachHangID AS UserID,
                HoTen,
                Email,
                'KhachHang' AS UserType,
                IsDeleted
            FROM 
                KhachHang
            WHERE 
                IsDeleted = 0";

            var queryAdmin = @"
            SELECT 
                AdminID AS UserID,
                HoTen,
                Email,
                'Admin' AS UserType,
                IsDeleted
            FROM 
                Admin
            WHERE 
                IsDeleted = 0";

            var queryShipper = @"
            SELECT 
                ShipperID AS UserID,
                HoTen,
                Email,
                'Shipper' AS UserType,
                IsDeleted
            FROM 
                Shipper
            WHERE 
                IsDeleted = 0";

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                queryKhachHang += " AND (HoTen LIKE @Search OR Email LIKE @Search)";
                queryAdmin += " AND (HoTen LIKE @Search OR Email LIKE @Search)";
                queryShipper += " AND (HoTen LIKE @Search OR Email LIKE @Search)";
            }

            var users = new List<object>();

            connection.Open();

            using (var command = new SqlCommand(queryKhachHang, connection))
            {
                if (!string.IsNullOrEmpty(queryParams.Search))
                {
                    command.Parameters.AddWithValue("@Search", "%" + queryParams.Search + "%");
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new
                        {
                            UserID = reader["UserID"],
                            HoTen = reader["HoTen"],
                            Email = reader["Email"],
                            UserType = reader["UserType"],
                            IsDeleted = reader["IsDeleted"]
                        };
                        users.Add(user);
                    }
                }
            }

            using (var command = new SqlCommand(queryAdmin, connection))
            {
                if (!string.IsNullOrEmpty(queryParams.Search))
                {
                    command.Parameters.AddWithValue("@Search", "%" + queryParams.Search + "%");
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new
                        {
                            UserID = reader["UserID"],
                            HoTen = reader["HoTen"],
                            Email = reader["Email"],
                            UserType = reader["UserType"],
                            IsDeleted = reader["IsDeleted"]
                        };
                        users.Add(user);
                    }
                }
            }

            using (var command = new SqlCommand(queryShipper, connection))
            {
                if (!string.IsNullOrEmpty(queryParams.Search))
                {
                    command.Parameters.AddWithValue("@Search", "%" + queryParams.Search + "%");
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new
                        {
                            UserID = reader["UserID"],
                            HoTen = reader["HoTen"],
                            Email = reader["Email"],
                            UserType = reader["UserType"],
                            IsDeleted = reader["IsDeleted"]
                        };
                        users.Add(user);
                    }
                }
            }

            connection.Close();
            return Ok(users);
        }
    }
    [Authorize]
    [HttpGet("danh-sach-shipper")]
    public IActionResult DanhSachShipper([FromQuery] SearchQueryParams queryParams)
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = @"
        SELECT 
            ShipperID,
            HoTen,
            Email,

            IsDeleted
        FROM 
            Shipper
        WHERE 
            IsDeleted = 0";

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query += " AND (HoTen LIKE @Search OR Email LIKE @Search)";
            }

            var command = new SqlCommand(query, connection);

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                command.Parameters.AddWithValue("@Search", "%" + queryParams.Search + "%");
            }

            connection.Open();
            var reader = command.ExecuteReader();
            var shipperList = new List<object>();
            while (reader.Read())
            {
                var shipper = new
                {
                    ShipperID = reader["ShipperID"],
                    HoTen = reader["HoTen"],
                    Email = reader["Email"],
              
                    IsDeleted = reader["IsDeleted"]
                };
                shipperList.Add(shipper);
            }
            connection.Close();
            return Ok(shipperList);
        }
    }
    [Authorize]
    [HttpGet("tat-ca-don-hang")]
    public IActionResult LayTatCaDonHang([FromQuery] SearchQueryParams1 queryParams)
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
            kh.HoTen AS HoTenKhachHang
        FROM 
            DonHang dh
        LEFT JOIN 
            TrangThaiDonHang tt ON dh.TrangThaiID = tt.TrangThaiID
        LEFT JOIN
            KhachHang kh ON dh.KhachHangID = kh.KhachHangID
        WHERE 
            dh.IsDeleted = 0";

            if (queryParams.TrangThaiId.HasValue)
            {
                query += " AND dh.TrangThaiID = @TrangThaiID";
            }

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query += " AND (dh.DiaChiNhanHang LIKE @Search OR dh.DiaChiGiaoHang LIKE @Search OR dh.SoDienThoaiNguoiNhan LIKE @Search OR dh.SoDienThoaiNguoiGui LIKE @Search OR kh.HoTen LIKE @Search)";
            }

            var command = new SqlCommand(query, connection);

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
                    ShipperID = reader["ShipperID"],
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
    [HttpGet("tat-ca-trang-thai")]
    public IActionResult LayTatCaTrangThaiDonHang()
    {
        using (var connection = new SqlConnection(GetConnectionString()))
        {
            var query = @"
        SELECT 
            TrangThaiID,
            MoTaTrangThai,
            IsDeleted
        FROM 
            TrangThaiDonHang
        WHERE 
            IsDeleted = 0";

            var command = new SqlCommand(query, connection);

            connection.Open();
            var reader = command.ExecuteReader();
            var trangThaiList = new List<object>();
            while (reader.Read())
            {
                var trangThai = new
                {
                    TrangThaiID = reader["TrangThaiID"],
                    MoTaTrangThai = reader["MoTaTrangThai"],
                    IsDeleted = reader["IsDeleted"]
                };
                trangThaiList.Add(trangThai);
            }
            connection.Close();
            return Ok(trangThaiList);
        }
    }

}
public class HoanHangDangXuLyQueryParams
{
    public string Search { get; set; } = "";
}
public class DonHangChoDuyetQueryParams
{
    public string Search { get; set; } = "";
}
public class SearchQueryParams
{
    public string Search { get; set; } = "";
}
