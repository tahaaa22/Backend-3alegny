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
    }
}

public class patientPHR<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}