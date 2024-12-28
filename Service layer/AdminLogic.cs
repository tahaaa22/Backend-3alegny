using _3alegny.Entities;
using _3alegny.RepoLayer;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;
using MongoDB.Bson;
using static AdminEndpoints;
using Microsoft.AspNetCore.Identity;

namespace _3alegny.Service_layer
{
    public class AdminLogic
    {
        private readonly MongoDbContext _context;

        public AdminLogic(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<AdminResult<T>> CreateBusiness<T>(BusinessRequest request, UserRole role) where T : User, new()
        {
            var collection = role switch
            {
                UserRole.Hospital => (IMongoCollection<T>)_context.Hospitals,
                UserRole.Pharmacy => (IMongoCollection<T>)_context.Pharmacies,
                _ => throw new ArgumentException("Invalid user role", nameof(role))
            };

            var filter = Builders<T>.Filter.Eq(u => u.UserName, request.UserName);

            var existingEntity = await collection.Find(filter).FirstOrDefaultAsync();
            if (existingEntity != null)
            {
                return new AdminResult<T>
                {
                    IsSuccess = false,
                    Data = existingEntity,
                    Role = existingEntity.Role.ToString(),
                    Message = "Error: duplicate entity"
                };
            }

            var entity = new T
            {
                Name = request.Name,
                UserName = request.UserName,
                Role = role,
                Password = HashPassword(request.Password),
                contactInfo = request.contactInfo,
                Address = request.Address,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                DeletedAt = request.CreatedAt
            };

            await collection.InsertOneAsync(entity);

            return new AdminResult<T>
            {
                IsSuccess = true,
                Data = entity,
                Role = entity.Role.ToString(),
                Message = $"{typeof(T).Name} {entity.Name} created successfully"
            };
        }



        private string HashPassword(string password)
        {
            var hasher = new PasswordHasher<string>();
            return hasher.HashPassword(null, password);
        }

        // Get all Patients
        public async Task<AdminResult<List<Patient>>> GetAllUsers()
        {
            try
            {
                var Patients = await _context.Patients.Find(_ => true).ToListAsync(); // Get all Patients
                if (Patients == null || !Patients.Any())
                {
                    return new AdminResult<List<Patient>> { IsSuccess = false, Message = "No Patients found." };
                }
                return new AdminResult<List<Patient>> { IsSuccess = true, Data = Patients , Message = "all users returned"};
            }
            catch (Exception ex)
            {
                return new AdminResult<List<Patient>> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        // Get a user by ID
        public async Task<AdminResult<User>> GetUserById(string id)
        {
            try
            {
                var objectId = new ObjectId(id); // Convert string ID to MongoDB ObjectId
                var user = await _context.Patients.Find(u => u.Id == objectId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new AdminResult<User> { IsSuccess = false, Message = "User not found." };
                }
                return new AdminResult<User> { IsSuccess = true, Data = user, Message = $"user with {id} valid" };
            }
            catch (Exception ex)
            {
                return new AdminResult<User> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        // Delete a user by ID
        public async Task<AdminResult<string>> DeleteUser(string id)
        {
            try
            {
                var objectId = new ObjectId(id); // Convert string ID to MongoDB ObjectId
                var deleteResult = await _context.Patients.DeleteOneAsync(u => u.Id == objectId);
                if (deleteResult.DeletedCount == 0)
                {
                    return new AdminResult<string> { IsSuccess = false, Message = "User not found." };
                }
                return new AdminResult<string> { IsSuccess = true, Message = "User deleted successfully." };
            }
            catch (Exception ex)
            {
                return new AdminResult<string> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}




public class AdminResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Role { get; set; }
    public required string Message { get; set; }
}