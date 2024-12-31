using _3alegny.Entities;
using _3alegny.RepoLayer;
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
                DeletedAt = request.CreatedAt,
                ImageUrl= request.imageUrl
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

        // Get all Users
        public async Task<AdminResult<List<int>>> GetAllUsers()
        {
            try
            {
                // Get all Patients
                var Patients = await _context.Patients.Find(_ => true).ToListAsync(); 
                if (Patients == null || !Patients.Any())
                {
                    return new AdminResult<List<int>> { IsSuccess = false, Message = "No Patients found." };
                }
                //Count the number of patients as a number
                var countPatients = await _context.Patients.CountDocumentsAsync(_ => true);





                //Count the number of hospitals
                var countHospitals = await _context.Hospitals.CountDocumentsAsync(_ => true);

                //Count the number of pharmacies
                var countPharmacies = await _context.Pharmacies.CountDocumentsAsync(_ => true);


                //Return the count of each entity as data 
                return new AdminResult<List<int>> { IsSuccess = true, Data = new List<int> { (int)countPatients, (int)countHospitals, (int)countPharmacies }, Message = "All users found." };



            }
            catch (Exception ex)
            {
                return new AdminResult<List<int>> { IsSuccess = false, Message = $"Error: {ex.Message}" };
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