﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SWP_Final.Entities;
using SWP_Final.Models;
using SWP_Final.Repositories;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace SWP_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepositories _userRepo;
        private string fileNameImageAcenciesModel;
        private readonly RealEasteSWPContext _context;

        public UsersController(IUserRepositories repository, RealEasteSWPContext context)
        {
            _userRepo = repository;
            _context = context;
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


        //POST: api/user/UploadUserById
        [HttpPost("UploadUserById/{userId}")]
        public async Task<IActionResult> UploadUserById([FromForm] UserModel userModel, string userId)
        {
            
            var user = await _context.Users.FindAsync(userId);
           
            if (user == null)
            {
                return NotFound("Agency not found");
            }
            user.Username = userModel.Username; 
            user.Password = userModel.Password;



            
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpGet("GetUserByUsername/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return NotFound(); // Trả về 404 nếu không tìm thấy người dùng
                }

                if (user.RoleId == "Agency")
                {
                    // Nếu người dùng là agency, trả về UserAccountModel với username, password và phone của class agency
                    var agency = await _context.Agencies.FirstOrDefaultAsync(a => a.UserId == user.UserId);
                    if (agency == null)
                    {
                        return NotFound("Agency information not found");
                    }

                    var userAccount = new UserAccountModel
                    {
                        Username = user.Username,
                        Password = user.Password,
                        Phone = agency.Phone // Phone của agency
                    };

                    return Ok(userAccount);
                }
                else if (user.RoleId == "Customer")
                {
                    // Nếu người dùng là customer, trả về UserAccountModel với username và password của class customer
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.UserId);
                    if (customer == null)
                    {
                        return NotFound("Customer information not found");
                    }

                    var userAccount = new UserAccountModel
                    {
                        Username = user.Username,
                        Password = user.Password,
                        Phone = customer.Phone // Phone của customer
                    };

                    return Ok(userAccount);
                }
                else
                {
                    return BadRequest("Invalid user role"); // Nếu vai trò không hợp lệ
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
