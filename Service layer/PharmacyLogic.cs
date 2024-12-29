
using _3alegny.Entities;
using _3alegny.RepoLayer;
using MongoDB.Bson;
using MongoDB.Driver;

namespace _3alegny.Service_layer
{
    public class PharmacyLogic
    {
        private readonly MongoDbContext _context;

        public PharmacyLogic(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<PharmacyResult> GetPharmacyById(string id)
        {
            try
            {
                var objectId = new ObjectId(id); // Convert string ID to MongoDB ObjectId
                var user = await _context.Pharmacies.Find(u => u.Id == objectId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new PharmacyResult { IsSuccess = false, Message = "hospital not found." };
                }
                return new PharmacyResult { IsSuccess = true, Data = user, Message = $"hospital with {id} valid" };
            }
            catch (Exception ex)
            {
                return new PharmacyResult { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }


        public class PharmacyResult
        {
            public bool IsSuccess { get; set; }
            public Pharmacy? Data { get; set; }
            public string? Role { get; set; }
            public required string Message { get; set; }
        }
    }
}
