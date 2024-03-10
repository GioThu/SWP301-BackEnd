using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SWP_Final.Entities;
using SWP_Final.Models;

namespace SWP_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly RealEasteSWPContext _context; 

        public LoginController(RealEasteSWPContext context)
        {
            _context = context;
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Đơn giản là trả về một phản hồi thành công khi người dùng logout
            return Ok("Logout successful");
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                // Kiểm tra thông tin đăng nhập trong cơ sở dữ liệu
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginModel.Username && u.Password == loginModel.Password);

                if (user == null)
                {
                    return NotFound("Invalid username or password");
                }

                // Tạo các claim cho mã token JWT
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                    new Claim("RoleId", user.RoleId),
                    // Thêm các claim khác tùy thuộc vào yêu cầu của bạn
                };

                // Nếu vai trò là Agency, thêm claim AgencyId
                if (user.RoleId == "Agency")
                {
                    var agency = await _context.Agencies.FirstOrDefaultAsync(a => a.UserId == user.UserId);
                    if (agency != null)
                    {
                        claims.Add(new Claim("AgencyId", agency.AgencyId));
                        claims.Add(new Claim("AgencyFirstName", agency.FirstName));
                        claims.Add(new Claim("AgencyLastName", agency.FirstName));
                    }
                }
                // Nếu vai trò là Customer, thêm claim CustomerId
                else if (user.RoleId == "Customer")
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.UserId);
                    if (customer != null)
                    {
                        claims.Add(new Claim("CustomerId", customer.CustomerId));
                        claims.Add(new Claim("CustomerFirstName", customer.FirstName));
                        claims.Add(new Claim("CustomerLastName", customer.FirstName));
                    }
                }

                // Tạo mã token JWT
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: "your_issuer_here",
                    audience: "your_audience_here",
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

                // Trả về token trong phản hồi
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
