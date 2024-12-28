using _3alegny.Entities;
using _3alegny.RepoLayer;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using static UserEndpoints;
using MongoDB.Bson;
using UserLoginRequest = UserEndpoints.LoginRequest;



namespace _3alegny.Service_layer
{
    public class UserLogic
    {
        private readonly MongoDbContext _context;

        public UserLogic(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message)> Signup(SignupRequest request)
        {
            // Check if passwords match
            if (request.CreatePassword != request.ConfirmPassword)
                return (false, "Passwords do not match.");

            // Check if username already exists
            var existingUser = await _context.Patients.Find(Builders<Patient>.Filter.Eq(u => u.UserName, request.UserName)).FirstOrDefaultAsync();

            if (existingUser != null)
                return (false, "Username already exists.");

            // Create the user
            // TODO: fix patient obj and change it to user until verification
            var user = new Patient
            {
                Name = request.Name,
                Gender = request.Gender,
                contactInfo = new ContactInfo
                {
                    Phone = request.ContactInfo // Assuming ContactInfo is just a phone number for now
                },
                Address = new Address
                {
                    Street = request.Address // Assuming Address is a single line for now
                },

                UserName = request.UserName,
                Password = HashPassword(request.CreatePassword), // Use a hashing function
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.Patient // Default role
            };

            await _context.Patients.InsertOneAsync(user);

            return (true, "Signup successful.");
        }

        public async Task<(bool IsSuccess, string Message)> Login(UserLoginRequest request)

        {
            // Find user by username
            var user = await _context.Patients.Find(u => u.UserName == request.UserName).FirstOrDefaultAsync();
            if (user == null)
                return (false, "Invalid username or password.");

            // Verify password
            if (!VerifyPassword(request.Password, user.Password))
                return (false, "Invalid username or password.");

            return (true, "Login successful.");
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
    }
}