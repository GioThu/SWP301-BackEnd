using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWP_Final.Models;
using SWP_Final.Repositories;

namespace SWP_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepositories _userRepo;

        public UsersController(IUserRepositories repository)
        {
            _userRepo = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepo.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userRepo.GetUserByIdAsync(id);
                return user == null ? NotFound() : Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userRepo.DeleteUserAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserModel userModel)
        {
            try
            {
                await _userRepo.UpdateUserAsync(userModel, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel registerModel)
        {
            string fileNameImageAcenciesModel = "Images/CustomersImage/" + registerModel.FileImage.FileName;

            try
            {
                // Lấy hình ảnh từ registerModel
                var image = registerModel.FileImage.FileName;

                // Thực hiện đăng ký sử dụng thông tin từ registerModel
                await _userRepo.RegisterAsync(registerModel.FirstName, registerModel.LastName,
                    registerModel.Phone, registerModel.Address, registerModel.Gender,
                    registerModel.Username, registerModel.Password, fileNameImageAcenciesModel);

                return CreatedAtAction(nameof(GetUserById), new { id = registerModel.Username }, registerModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]LoginModel loginModel)
        {
            try
            {
                var user = await _userRepo.LoginAsync(loginModel.Username, loginModel.Password);
                return user == null ? NotFound("Invalid username or password") : Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
