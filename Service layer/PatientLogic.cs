using MongoDB.Bson;
using MongoDB.Driver;
using _3alegny.Entities;
using _3alegny.RepoLayer;
using static PatientEndpoints;
using System.CodeDom.Compiler;
using static _3alegny.Service_layer.HospitalLogic;
using System.Numerics;

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

        // Update patient info by ID
        public async Task<PatientResult<Patient>> UpdatePatient(string id, PatientUpdateRequest patient)
        {
            try
            {
                var objectId = new ObjectId(id); // Convert string ID to MongoDB ObjectId
                var updateResult = await _context.Patients.UpdateOneAsync
                    (
                        u => u.Id == objectId,
                        Builders<Patient>.Update
                        .Set(p => p.UserName, patient.UserName)
                        .Set(p => p.contactInfo.Email, patient.Email)
                        .Set(p => p.contactInfo.Phone, patient.Phone)
                        .Set(p => p.Address.Street, patient.Street)
                        .Set(p => p.Address.City, patient.City)
                        .Set(p => p.Address.State, patient.State)
                        .Set(p => p.Address.ZipCode, patient.ZipCode)
                        .Set(p => p.ImageUrl, patient.imageUrl));
                var patientRecord = await _context.Patients.Find(u => u.Id == objectId).FirstOrDefaultAsync();

                if (patientRecord.Insurance != null)
                {
                    var existingInsurance = patientRecord.Insurance.FirstOrDefault(ins => ins.providerName == patient.InsuranceName);

                    if (existingInsurance != null)
                    {
                        return new PatientResult<Patient> { IsSuccess = false, Message = "insurance providerName already exists" };
                    }
                    else
                    {
                        // Add new insurance record
                        var newInsurance = new Insurance
                        {
                            Id = ObjectId.GenerateNewId(),
                            providerName = patient.InsuranceName
                        };

                        patientRecord.Insurance.Add(newInsurance);
                    }

                    // Update the patient's insurance list in the database
                    var updateInsuranceResult = await _context.Patients.UpdateOneAsync(
                        u => u.Id == objectId,
                        Builders<Patient>.Update.Set(p => p.Insurance, patientRecord.Insurance)
                    );
                }
                else
                {
                    var InsuranceList = new List<Insurance>();
                    var insurance = new Insurance { Id = ObjectId.GenerateNewId(), providerName = patient.InsuranceName };
                    InsuranceList.Add(insurance);
                    var updateInsuranceResult = await _context.Patients.UpdateOneAsync(
                        u => u.Id == objectId,
                        Builders<Patient>.Update.Set(p => p.Insurance, InsuranceList)
                    );
                    return new PatientResult<Patient> { IsSuccess = true, Message = "Patient updated successfully." };

                }


                    return updateResult.ModifiedCount == 0
                    ? new PatientResult<Patient> { IsSuccess = false, Message = "Patient update failed." }
                    : new PatientResult<Patient> { IsSuccess = true, Message = "Patient updated successfully." };

            }
            catch (Exception ex)
            {
                return new PatientResult<Patient> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }



        //get the top doctor in the current department
        public async Task<Doctors> GetTopDoctor(string departmentId, string hospitalId)
        {
            try
            {
                var hospitalObjectId = new ObjectId(hospitalId);
                var departmentObjectId = new ObjectId(departmentId);

                // Find the hospital that contains the department
                var hospital = await _context.Hospitals
                    .Find(h => h.Id == hospitalObjectId && h.Departments.Any(d => d.Id == departmentObjectId))
                    .FirstOrDefaultAsync();

                if (hospital == null)
                {
                    throw new Exception($"Hospital with ID {hospitalId} not found or department missing.");
                }

                // Retrieve the department
                var department = hospital.Departments.FirstOrDefault(d => d.Id == departmentObjectId);
                if (department == null)
                {
                    return null; // No matching department found
                }

                // Find top doctor in the department
                var topDoctor = hospital.Doctors
                    .Where(d => d.Specialty == department.Name) // Match specialty to department name
                    .OrderByDescending(d => d.Rating)           // Assuming "Rating" is a property of Doctor
                    .FirstOrDefault();

                return topDoctor; // Return the top doctor or null if none found
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception($"Error occurred while retrieving the top doctor: {ex.Message}");
            }
        }


        // Create a new PHR for a patient
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
                    BMI = new List<int> { phr.BMI }
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


        public async Task<AdminResult<List<Hospital>>> GetAllHospitals()
        {
            try
            {
                var hospital = await _context.Hospitals.Find(_ => true).ToListAsync();
                if (hospital == null || !hospital.Any())
                {
                    return new AdminResult<List<Hospital>> { IsSuccess = false, Message = "No Patients found." };
                }
                return new AdminResult<List<Hospital>> { IsSuccess = true, Data = hospital, Message = "all users returned" };
            }
            catch (Exception ex)
            {
                return new AdminResult<List<Hospital>> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        // Get Followup for patient
        public async Task<FollowUpResult<List<FollowUp>>> GetFollowUp(string id)
        {
            try
            {
                var objectId = new ObjectId(id); // Convert string ID to MongoDB ObjectId
                var user = await _context.Patients.Find(u => u.Id == objectId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new FollowUpResult<List<FollowUp>> { IsSuccess = false, Message = "patient not found." };
                }
                return new FollowUpResult<List<FollowUp>> { IsSuccess = true, Data = user.FollowUp, Message = $"patient with {id} valid" };
            }
            catch (Exception ex)
            {
                return new FollowUpResult<List<FollowUp>> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }



        public async Task<PatientResult<List<Hospital>>> FilterByLocation(List<Hospital> hospitals)
        {
            try
            {
                return new PatientResult<List<Hospital>>
                {
                    IsSuccess = false,
                    Message = $"Error: {"testing"}"
                };

            }
            catch (Exception ex)
            {
                return new PatientResult<List<Hospital>>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}"
                };

            }

        }


        public async Task<PatientResult<List<Hospital>>> GetAvailableHospitals(HospitalFiltrationRequest<Hospital> FilteredHospital)
        {
            try
            {
                // Fetch all hospitals initially
                var hospitals = await _context.Hospitals.Find(_ => true).ToListAsync();

                if (hospitals == null || !hospitals.Any())
                {
                    return new PatientResult<List<Hospital>>
                    {
                        IsSuccess = false,
                        Message = "No hospitals found."
                    };
                }

                // Apply dynamic filtering
                if (!string.IsNullOrEmpty(FilteredHospital.street) && FilteredHospital.street != "string")
                {
                    hospitals = hospitals.FindAll(h => h.Address.City == FilteredHospital.street);
                }

                if (!string.IsNullOrEmpty(FilteredHospital.rating) && FilteredHospital.rating != "string")
                {
                    if (double.TryParse(FilteredHospital.rating, out double rating))
                    {
                        hospitals = hospitals.FindAll(h => h.Rating >= rating);
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

                if (!string.IsNullOrEmpty(FilteredHospital.department) && FilteredHospital.department != "string")
                {
                    hospitals = hospitals.FindAll(h => h.Departments.Any(d => d.Name == FilteredHospital.department));
                }

                if (FilteredHospital.price != 0)
                { 
                        var finalFilteredHospitals = hospitals.FindAll(h =>
                            h.Departments.Any(d =>
                                d.AvaliableDoctors != null) &&
                                h.Departments.Any(d => d.AppointmentFee <= FilteredHospital.price));

                  if(finalFilteredHospitals == null || !finalFilteredHospitals.Any())
                    return new PatientResult<List<Hospital>>
                    {
                        IsSuccess = false,
                        Message = "Invalid price value."
                    };
                
                }

                // Final check for results
                if (hospitals == null || !hospitals.Any())
                {
                    return new PatientResult<List<Hospital>>
                    {
                        IsSuccess = false,
                        Message = "No hospitals found matching the given filters."
                    };
                }

                return new PatientResult<List<Hospital>>
                {
                    IsSuccess = true,
                    Data = hospitals,
                    Message = "Filtered hospitals returned successfully."
                };
            }
            catch (Exception ex)
            {
                return new PatientResult<List<Hospital>>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}"
                };
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

public class FollowUpResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; }
}   
public class FilteredHospitalResponse
{
    public bool IsSuccess { get; set; }
    public string? address { get; set; }
    public string? rating { get; set; }
    public string? department { get; set; }
    public string? price { get; set; }
    public string? Role { get; set; }
    public required string Message { get; set; }

}