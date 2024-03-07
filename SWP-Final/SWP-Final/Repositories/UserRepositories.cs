using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP_Final.Entities;
using SWP_Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWP_Final.Repositories
{
    public class UserRepositories : IUserRepositories
    {
        private readonly RealEasteSWPContext _context;
        private readonly IMapper _mapper;

        public UserRepositories(RealEasteSWPContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task DeleteUserAsync(string id)
        {
            // Tìm user cần xóa
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User not found.");
            }

            // Xoá customer có UserId tương ứng (nếu có)
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            // Xoá agency có UserId tương ứng (nếu có)
            var agency = await _context.Agencies.FirstOrDefaultAsync(a => a.UserId == id);
            if (agency != null)
            {
                _context.Agencies.Remove(agency);
            }

            // Xoá user
            _context.Users.Remove(user);

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
        }




        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<UserModel>>(users);
        }

        public async Task<UserModel> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            return _mapper.Map<UserModel>(user);
        }

        public async Task<UserModel> GetUserByNameAsync(string name)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == name);
            return _mapper.Map<UserModel>(user);
        }

        public async Task UpdateUserAsync(UserModel userModel, string userID)
        {
            var user = await _context.Users.FindAsync(userID);
            if (user != null)
            {
                _mapper.Map(userModel, user);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserModel> LoginAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username && u.Password == password);
            return _mapper.Map<UserModel>(user);
        }

        public async Task RegisterAsync(string firstName, string lastName, string phone,
    string address, string gender, string username, string password,
    string image)
        {
            // Check if the username already exists to avoid duplicates
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                throw new Exception("Username already exists."); // Use a more specific exception for real-world applications
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid().ToString(), // Automatically generate a new GUID for the UserId
                Username = username,
                Password = password,
                RoleId = "Customer" // Set the default RoleId as "Customer"
                                    // Add other default values or fields as necessary
            };

            var newCustomer = new Customer
            {
                CustomerId = Guid.NewGuid().ToString(), // Automatically generate a new GUID for the CustomerId
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                Gender = gender,
                Images = image,
                UserId = newUser.UserId,
                Phone = phone
                // Add other properties as necessary
            };

            await _context.Users.AddAsync(newUser);
            await _context.Customers.AddAsync(newCustomer);
            await _context.SaveChangesAsync();
        }

        public async Task RegisterAsyncWithNoImage(string firstName, string lastName, string phone, string address, string gender, string username, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                throw new Exception("Username already exists."); // Use a more specific exception for real-world applications
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid().ToString(), // Automatically generate a new GUID for the UserId
                Username = username,
                Password = password,
                RoleId = "Customer" // Set the default RoleId as "Customer"
                                    // Add other default values or fields as necessary
            };

            var newCustomer = new Customer
            {
                CustomerId = Guid.NewGuid().ToString(), // Automatically generate a new GUID for the CustomerId
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                Gender = gender,
                Images = null,
                UserId = newUser.UserId,
                Phone = phone
                // Add other properties as necessary
            };

            await _context.Users.AddAsync(newUser);
            await _context.Customers.AddAsync(newCustomer);
            await _context.SaveChangesAsync();
        }
    }
}
