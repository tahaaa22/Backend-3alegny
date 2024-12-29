using MongoDB.Bson;
using MongoDB.Driver;
using _3alegny.Entities;
using _3alegny.RepoLayer;
using static PatientEndpoints;

namespace _3alegny.Service_layer
{
    public class PatientLogic
    {
        private readonly MongoDbContext _context;

        public PatientLogic(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<PatientResult<Patient>> GetPatientById(string id)
        {
            try
            {
                var objectId = new ObjectId(id); // Convert string ID to MongoDB ObjectId
                var user = await _context.Patients.Find(u => u.Id == objectId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new PatientResult<Patient> { IsSuccess = false, Message = "patient not found." };
                }
                return new PatientResult<Patient> { IsSuccess = true, Data = user, Message = $"patient with {id} valid" };
            }
            catch (Exception ex)
            {
                return new PatientResult<Patient> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<patientPHR<string>> PostPHR(phrRequest phr)
        {
            try
            {
                // Map PostPHR to PHR
                var PHR = new PHR
                {
                    Allergies = phr.Allergies,
                    ChronicIllness = phr.ChronicIllness,
                    Diagnosis = phr.Diagnosis,
                    Medication = phr.Medication,
                    FamilyHistory = phr.FamilyHistory,
                    ImagingResults = phr.ImagingResults,
                    LabResults = phr.LabResults,
                    MedicalProcedures = phr.MedicalProcedures,
                    PrescriptionHistory = phr.PrescriptionHistory
                };

                await _context.PHRs.InsertOneAsync(PHR);
                return new patientPHR<string> { IsSuccess = true, Message = "PHR uploaded successfully." };
            }
            catch (Exception ex)
            {
                return new patientPHR<string> { IsSuccess = false, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<patientPHR<string>> UpdatePHR(string id, phrRequest updatedPhr)
        {
            try
            {
                // Parse the ObjectId
                var objectId = ObjectId.Parse(id);

                // Define the filter to locate the document by ID
                var filter = Builders<PHR>.Filter.Eq(p => p.PHRId, objectId);

                // Define the update operation
                var update = Builders<PHR>.Update
                    .Set(p => p.Allergies, updatedPhr.Allergies)
                    .Set(p => p.ChronicIllness, updatedPhr.ChronicIllness)
                    .Set(p => p.Diagnosis, updatedPhr.Diagnosis)
                    .Set(p => p.Medication, updatedPhr.Medication)
                    .Set(p => p.FamilyHistory, updatedPhr.FamilyHistory)
                    .Set(p => p.ImagingResults, updatedPhr.ImagingResults)
                    .Set(p => p.LabResults, updatedPhr.LabResults)
                    .Set(p => p.MedicalProcedures, updatedPhr.MedicalProcedures)
                    .Set(p => p.PrescriptionHistory, updatedPhr.PrescriptionHistory);

                var result = await _context.PHRs.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                    return new patientPHR<string> { IsSuccess = false, Message = "PHR not found." };

                return new patientPHR<string> { IsSuccess = true, Message = "PHR updated successfully." };
            }
            catch (Exception ex)
            {
                return new patientPHR<string> { IsSuccess = false, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<patientPHR<PHR>> GetPHR(string id)
        {
            try
            {
                var phrId = new ObjectId(id);
                var phr = await _context.PHRs.Find(u => u.PHRId == phrId).FirstOrDefaultAsync();
                if (phr == null)
                {
                    return new patientPHR<PHR> { IsSuccess = false, Message = "User not found." };
                }
                return new patientPHR<PHR> { IsSuccess = true, Data = phr };
            }
            catch (Exception e)
            {
                return new patientPHR<PHR> { IsSuccess = false, Message = $"Error: {e.Message}" };
            }
        }

        public async Task<PatientResult<List<Hospital>>> GetAvailableHospitals(HospitalFiltrationRequest<Hospital> FilteredHospital)
        {
            try
            {
                // Start with the hospital collection query
                var query = _context.Hospitals.AsQueryable();

                // Filter by street (default to a specific value if empty)
                if (string.IsNullOrEmpty(FilteredHospital.street))
                {
                    query = query.Where(h => h.Address.Street == "DefaultStreet");
                }
                else
                {
                    query = query.Where(h => h.Address.Street == FilteredHospital.street);
                }

                // Filter by price (if provided)
                if (!string.IsNullOrEmpty(FilteredHospital.price))
                {
                    if (double.TryParse(FilteredHospital.price, out double price))
                    {
                        query = query.Where(h => h.Doctors.Any(d => d.AppointmentFee <= price));
                    }
                    else
                    {
                        return new PatientResult<List<Hospital>>
                        {
                            IsSuccess = false,
                            Message = "Invalid price value."
                        };
                    }
                }

                // Filter by department (if provided)
                if (!string.IsNullOrEmpty(FilteredHospital.department))
                {
                    query = query.Where(h => h.Departments.Any(d => d.Name == FilteredHospital.department));
                }

                // Filter by rating (if provided)
                if (!string.IsNullOrEmpty(FilteredHospital.rating))
                {
                    if (double.TryParse(FilteredHospital.rating, out double rating))
                    {
                        query = query.Where(h => h.Rating >= rating);
                    }
                    else
                    {
                        return new PatientResult<List<Hospital>>
                        {
                            IsSuccess = false,
                            Message = "Invalid rating value."
                        };
                    }
                }

                // Execute the query and get the result list
                var hospitals = await _context.Hospitals.Find(h=> h.Equals(query)).ToListAsync();

                if (!hospitals.Any())
                {
                    return new PatientResult<List<Hospital>> { IsSuccess = false, Message = "No hospitals found with the given filters." };
                }

                return new PatientResult<List<Hospital>>
                {
                    IsSuccess = true,
                    Data = hospitals,
                    Message = $"Hospitals filtered for patient {FilteredHospital.PatientId}."
                };
            }
            catch (Exception ex)
            {
                return new PatientResult<List<Hospital>> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }
        public async Task<AdminResult<List<Pharmacy>>> GetAllPharmacies()
        {
            try
            {
                var pharmacy = await _context.Pharmacies.Find(_ => true).ToListAsync(); 
                if (pharmacy == null || !pharmacy.Any())
                {
                    return new AdminResult<List<Pharmacy>> { IsSuccess = false, Message = "No Patients found." };
                }
                return new AdminResult<List<Pharmacy>> { IsSuccess = true, Data = pharmacy, Message = "all users returned" };
            }
            catch (Exception ex)
            {
                return new AdminResult<List<Pharmacy>> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }





    }
}
public class PatientResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Role { get; set; }
    public required string Message { get; set; }
}

public class patientPHR<T> 
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
}