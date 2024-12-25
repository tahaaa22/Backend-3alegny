using _3alegny.Entities;
using _3alegny.RepoLayer;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
namespace _3alegny.Service_layer
{
    public class CommonLogic
    {
        private readonly MongoDbContext _context;

        public CommonLogic(MongoDbContext context)
        {
            _context = context;
        }

        // Method to get the top 4 hospitals by rating
        public async Task<List<TopHospital>> GetTopRatedHospitals()
        {
            

            // Query to find the top 4 hospitals sorted by rating in descending order
            var topHospitals = await _context.Hospitals.Find(_ => true).Sort(Builders<Hospital>.Sort.Descending(h => h.Rating)) // Sort by Rating
                .Limit(4)// Get only the top 4
                .Project(h => new TopHospital(
                    h.Name,
                    h.Rating ?? 0.0)) 
                .ToListAsync();

            return topHospitals;
        }

        // Method to get the top 4 doctors by rating
        public async Task<List<TopDoctor>> GetTopRatedDoctors()
        {
            // Aggregate query to get all doctors from all hospitals, sort them by rating, and pick the top 4
            var pipeline = new[]
            {
                new BsonDocument { { "$unwind", "$Doctors" } }, // Flatten the Doctors array
                new BsonDocument { { "$sort", new BsonDocument("Doctors.Rating", -1) } }, // Sort by Doctors' rating in descending order
                new BsonDocument { { "$limit", 4 } }, // Get the top 4 doctors
                new BsonDocument
                {
                    { "$project", new BsonDocument
                        {
                            { "_id", 0 }, // Exclude the _id field
                            { "Name", "$Doctors.Name" }, // Include the Doctor's name
                            { "Rating", "$Doctors.Rating" } // Include the Doctor's rating
                        }
                    }
                }
            };

            var topDoctors = await _context.Hospitals.Aggregate<TopDoctor>(pipeline).ToListAsync();

            return topDoctors;
        }

        // Method to get the top 4 pharmacies by rating
        public async Task<List<TopPharmacy>> GetTopRatedPharmacies()
        {
            // Query to find the top 4 pharmacies sorted by rating in descending order
            var topPharmacies = await _context.Pharmacies.Find(_ => true).Sort(Builders<Pharmacy>.Sort.Descending(p => p.Rating)) // Sort by Rating
                .Limit(4)// Get only the top 4
                .Project(p => new TopPharmacy(
                    p.Name,
                    p.Rating))
                .ToListAsync();

            return topPharmacies;
        }
        // Method to get the list of hospitals and the insurances they accept
        public async Task<List<HospitalInsurance>> GetHospitalInsurances()
        {
            var hospitals = await _context.Hospitals.Find(_ => true)
                .SortBy(h => h.Name)
                .ToListAsync();
            var hospitalInsurances = hospitals.Select(h => new HospitalInsurance(
                h.Name,
                h.InsuranceAccepted
            )).ToList();
            return hospitalInsurances;
        }
    }

    public record TopHospital(string Name, double Rating);
    public record TopDoctor(string Name, double Rating);
    public record TopPharmacy(string Name, double Rating);
    public record HospitalInsurance(string Name, List<Insurance> InsuranceAccepted);
}
