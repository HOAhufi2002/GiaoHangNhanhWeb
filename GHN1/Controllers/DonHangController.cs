using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace GHN1.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class DonHangController : ControllerBase
        {
            private readonly IConfiguration _configuration;

            public DonHangController(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            private string GetConnectionString()
            {
                return _configuration.GetConnectionString("DefaultConnection");
            }

            // Tra cứu trạng thái đơn hàng
            [HttpGet("tra-cuu-trang-thai/{donHangId}")]
            public IActionResult TraCuuTrangThaiDonHang(int donHangId)
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
                    dh.NgayTao,
                    dh.NgayCapNhat,
                    dh.IsDeleted
                FROM 
                    DonHang dh
                LEFT JOIN 
                    TrangThaiDonHang tt ON dh.TrangThaiID = tt.TrangThaiID
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
                            NgayTao = reader["NgayTao"],
                            NgayCapNhat = reader["NgayCapNhat"],
                            IsDeleted = reader["IsDeleted"]
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

    }

