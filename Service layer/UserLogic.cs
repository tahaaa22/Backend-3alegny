using _3alegny.Entities;
using _3alegny.RepoLayer;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using static UserEndpoints;
using MongoDB.Bson;
using UserLoginRequest = UserEndpoints.LoginRequest;
using System.Reflection;



namespace _3alegny.Service_layer
{
    public class UserLogic
    {
        private readonly MongoDbContext _context;

        public UserLogic(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<SignUpResponse> Signup(SignupRequest request)
        {

            // Check if username already exists
            var existingUser = await _context.Patients.Find(Builders<Patient>.Filter.Eq(u => u.UserName, request.UserName)).FirstOrDefaultAsync();

            if (existingUser != null)
                return new SignUpResponse { IsSuccess = false, Message = "Username already exists." };

            var user = new Patient
            {
                Name = request.Name,
                UserName = request.UserName,
                Role = UserRole.Patient,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Password = HashPassword(request.Password),
                contactInfo = new ContactInfo
                {
                    Phone = request.Phone,
                    Email = request.Email
                },
                Address = new Address
                {
                    Street = request.Street,
                    City = request.City,
                    State = request.state,
                    ZipCode = request.ZipCode
                },
                CreatedAt = DateTime.UtcNow
                 
            };

            await _context.Patients.InsertOneAsync(user);

            return new SignUpResponse
            {
                IsSuccess = true,
                UserName = user.UserName,
                ID = user.Id.ToString(),
                Role = user.Role.ToString(),
                Message = "User login succeeded."
            };
        }

        public async Task<LoginResponse> Login(UserLoginRequest request)
        {
            User user = await _context.Patients.Find(u => u.UserName == request.UserName).FirstOrDefaultAsync();

            if (user == null)
                user = await _context.Hospitals.Find(u => u.UserName == request.UserName).FirstOrDefaultAsync();

            if (user == null)
                user = await _context.Pharmacies.Find(u => u.UserName == request.UserName).FirstOrDefaultAsync();

            if (user == null)
                user = await _context.Admin.Find(u => u.UserName == request.UserName).FirstOrDefaultAsync();

            if (user == null)
            {
                return new LoginResponse { IsSuccess = false, Message = "User not found." };
            }

            if (!VerifyPassword(request.Password, user.Password))
            {
                return new LoginResponse { IsSuccess = false, Message = "Wrong username/password." };
            }

            return new LoginResponse
            {
                IsSuccess = true,
                UserName = user.UserName,
                ID = user.Id.ToString(),
                Role = user.Role.ToString(),
                Message = "User login succeeded."
            };
        }

        private string HashPassword(string password)
        {
            var hasher = new PasswordHasher<string>();
            return hasher.HashPassword(null, password);
        }

        private bool VerifyPassword(string providedPassword, string storedPassword)
        {
            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(null, storedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }

        public class SignUpResponse
        {
            public bool IsSuccess { get; set; }
            public string? UserName { get; set; }
            public string? ID { get; set; }
            public string? Role { get; set; }
            public string Message { get; set; }
        }
        public class LoginResponse
        {
            public bool IsSuccess { get; set; }
            public string? UserName { get; set; }
            public string? ID { get; set; }
            public string? Role { get; set; }
            public string Message { get; set; }
        }
    }
}