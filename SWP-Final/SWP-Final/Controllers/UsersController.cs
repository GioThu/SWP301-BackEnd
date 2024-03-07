using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWP_Final.Entities;
using SWP_Final.Models;
using SWP_Final.Repositories;

namespace SWP_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepositories _userRepo;
        private string fileNameImageAcenciesModel;

        public UsersController(IUserRepositories repository)
        {
            _userRepo = repository;
        }

        [HttpPut("BlockUser/{id}")]
        public async Task<IActionResult> BlockUser(string id)
        {
            try
            {
                await _userRepo.BlockUsers(id);
                return Ok("User's status has been updated successfully.");
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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
            if (registerModel.FileImage != null && registerModel.FileImage.Length > 0)
            {
                string filename = "Images/CustomerImages/"+registerModel.FileImage.FileName;

                var path = GetFilePath(filename);
                using (var stream = System.IO.File.Create(path))
                {
                    await registerModel.FileImage.CopyToAsync(stream);
                }
                fileNameImageAcenciesModel = filename;
            }

            try
            {
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

        [HttpPost("registerNoImage")]
        public async Task<IActionResult> RegisterWithNoImage([FromForm] RegisterNoImageModel registerModel)
        {
            
            try
            {
                // Thực hiện đăng ký sử dụng thông tin từ registerModel
                await _userRepo.RegisterAsyncWithNoImage(registerModel.FirstName, registerModel.LastName,
                    registerModel.Phone, registerModel.Address, registerModel.Gender,
                    registerModel.Username, registerModel.Password);

                return CreatedAtAction(nameof(GetUserById), new { id = registerModel.Username }, registerModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);


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
