using MongoDB.Bson;
using MongoDB.Driver;
using _3alegny.Entities;
using _3alegny.RepoLayer;
using static PatientEndpoints;
using System.CodeDom.Compiler;

namespace _3alegny.Service_layer
{
    public class PatientLogic
    {
        private readonly MongoDbContext _context;

        public PatientLogic(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<PatientResult> GetPatientById(string id)
        {
            try
            {
                var objectId = new ObjectId(id); // Convert string ID to MongoDB ObjectId
                var user = await _context.Patients.Find(u => u.Id == objectId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new PatientResult { IsSuccess = false, Message = "patient not found." };
                }
                return new PatientResult { IsSuccess = true, Data = user, Message = $"patient with {id} valid" };
            }
            catch (Exception ex)
            {
                return new PatientResult { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<patientPHR<string>> PostPHR(string pid, phrRequest phr)
        {
            try
            {
                // Map PostPHR to PHR
                var PHR = new PHR
                {
                    PatientId = pid,
                    Allergies = phr.Allergies,
                    ChronicIllness = phr.ChronicIllness,
                    Diagnosis = phr.Diagnosis,
                    Medication = phr.Medication,
                    FamilyHistory = phr.FamilyHistory,
                    ImagingResults = phr.ImagingResults,
                    LabResultsURL = phr.LabResultsURL,
                    MedicalProcedures = phr.MedicalProcedures,
                    PrescriptionHistory = phr.PrescriptionHistory,
                    Weight = new List<int> { phr.Weight },
                    Height = new List<int> { phr.Height },
                    BMI = new List<int>{phr.BMI}
                };

                await _context.PHRs.InsertOneAsync(PHR);
                await _context.Patients.UpdateOneAsync(
                            u => u.Id == ObjectId.Parse(pid),
                            Builders<Patient>.Update.Set(p => p.PHR, PHR)
                        );
                return new patientPHR<string> { IsSuccess = true, Message = "PHR uploaded successfully." };
            }
            catch (Exception ex)
            {
                return new patientPHR<string> { IsSuccess = false, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<patientPHR<PHR>> UpdatePHR(string pid, phrRequest updatedPhr)
        {
            try
            {
                // Parse the ObjectId
                var objectId = ObjectId.Parse(pid);

                // Find the patient by ID
                var patient = await _context.Patients.Find(p => p.Id == objectId).FirstOrDefaultAsync();
                if (patient == null)
                {
                    return new patientPHR<PHR> { IsSuccess = false, Message = "Patient not found." };
                }

                // Find the existing PHR by patient ID
                var existingPHR = await _context.PHRs.Find(p => p.PatientId == pid).FirstOrDefaultAsync();
                if (existingPHR == null)
                {
                    return new patientPHR<PHR> { IsSuccess = false, Message = "PHR not found." };
                }

                // Map updated PHR to existing PHR
                existingPHR.Allergies = updatedPhr.Allergies;
                existingPHR.ChronicIllness = updatedPhr.ChronicIllness;
                existingPHR.Diagnosis = updatedPhr.Diagnosis;
                existingPHR.Medication = updatedPhr.Medication;
                existingPHR.FamilyHistory = updatedPhr.FamilyHistory;
                existingPHR.ImagingResults = updatedPhr.ImagingResults;
                existingPHR.LabResultsURL = updatedPhr.LabResultsURL;
                existingPHR.MedicalProcedures = updatedPhr.MedicalProcedures;
                existingPHR.PrescriptionHistory = updatedPhr.PrescriptionHistory;
                existingPHR.Weight.Add(updatedPhr.Weight);
                existingPHR.Height.Add(updatedPhr.Height);
                existingPHR.BMI.Add(updatedPhr.BMI);

                // Update the PHR
                await _context.PHRs.ReplaceOneAsync(p => p.Id == existingPHR.Id, existingPHR);
                // Update the Patient's PHR reference
                var updatePatientResult = await _context.Patients.UpdateOneAsync(
                    p => p.Id == objectId,
                    Builders<Patient>.Update.Set(p => p.PHR, existingPHR)
                );
                if (updatePatientResult.ModifiedCount == 0)
                {
                    return new patientPHR<PHR> { IsSuccess = false, Message = "Patient update failed." };
                }

                return new patientPHR<PHR> { IsSuccess = true, Data = existingPHR, Message = "PHR and Patient updated successfully" };
               
            }
            catch (Exception ex)
            {
                return new patientPHR<PHR> { IsSuccess = false, Message = $"An error occurred: {ex.Message}" };
            }
        }
        //Get PHR by id
        public async Task<patientPHR<PHR>> GetPHR(string patientId)
        {
            try
            {
                // Find the patient by PatientId
                var patient = await _context.Patients.Find(p => p.Id == ObjectId.Parse(patientId)).FirstOrDefaultAsync();
                if (patient == null)    
                {
                    return new patientPHR<PHR> { IsSuccess = false, Message = "Patient not found." };
                }

                // Retrieve the PHR associated with the patient
                var phr = patient.PHR;
                if (phr == null)
                {
                    return new patientPHR<PHR> { IsSuccess = false, Message = "PHR not found for the given patient." };
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
public class PatientResult
{
    public bool IsSuccess { get; set; }
    public Patient? Data { get; set; }
    public string? Role { get; set; }
    public required string Message { get; set; }
}

public class patientPHR<T> 
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}