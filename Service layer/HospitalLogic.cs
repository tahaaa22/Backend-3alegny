﻿using _3alegny.Entities;
using _3alegny.RepoLayer;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using static HospitalEndpoints;
namespace _3alegny.Service_layer
{
    public class HospitalLogic
    {
        private readonly MongoDbContext _context;

        public HospitalLogic(MongoDbContext context)
        {
            _context = context;
        }

        // Method to add a department to a hospital
        public async Task<string> AddDepartment(string hospitalId, string departmentName)
        {   //Check if the hospital exists
            var hospital = await _context.Hospitals.Find(h => h.Id.ToString() == hospitalId).FirstOrDefaultAsync();
            if (hospital == null)
                throw new Exception("Hospital not found");

            var newDepartment = new Department { Name = departmentName };
            hospital.Departments.Add(newDepartment);

            await _context.Hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);
            return "Department added successfully";
        }

        public async Task<object> AddDoctor(DoctorResponse doctor)
        {
            if (string.IsNullOrEmpty(doctor.Id) || !ObjectId.TryParse(doctor.Id, out _))
            {
                doctor = doctor with { Id = ObjectId.GenerateNewId().ToString() };
            }

            var hospital = await _context.Hospitals.Find(h => h.Id.ToString() == doctor.HospitalId.ToString()).FirstOrDefaultAsync();
            if (hospital == null)
                throw new Exception("Hospital not found");

            var matchingDepartment = hospital.Departments.FirstOrDefault(dept => dept.Name.Equals(doctor.Specialty, StringComparison.OrdinalIgnoreCase));
            //if (matchingDepartment == null)
            //    throw new Exception("Department not found");

            var doctorEntity = new Doctors
            {
                Id = ObjectId.Parse(doctor.Id),
                Name = doctor.Name,
                Specialty = doctor.Specialty,
                HospitalId = doctor.HospitalId,
                License = doctor.License,
                Description = doctor.Description,
                address = new Address
                {
                    City = doctor.city,
                    State = doctor.state,
                    ZipCode = doctor.zipcode,
                    Street = doctor.street
                },

                AppointmentFee = doctor.AppointmentFee,
                ImageUrl = doctor.ImageUrl,
                AvailableSlots = doctor.AvailableSlots ?? new List<DateTime>()
               
            };


            hospital.Doctors.Add(doctorEntity);
            await _context.Hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);

            return new
            {
                Success = true,
                Message = "Doctor added successfully",
                Doctor = doctor
            };
        }




        public async Task<object> UpdateDoctorById(string doctorId, UpdateDoctorRequest updatedDoctor)
        {
            // Validate the doctorId
            if (!ObjectId.TryParse(doctorId, out _))
                throw new Exception("Invalid doctor ID");

            // Find the hospital that contains the doctor
            var hospital = await _context.Hospitals.Find(h => h.Doctors.Any(d => d.Id.ToString() == doctorId)).FirstOrDefaultAsync();
            if (hospital == null)
                throw new Exception("Hospital not found");

            // Find the doctor to update
            var doctor = hospital.Doctors.FirstOrDefault(d => d.Id.ToString() == doctorId);
            if (doctor == null)
                throw new Exception("Doctor not found");

            // Update doctor properties with the provided ones
            doctor.Name = updatedDoctor.Name ?? doctor.Name;
            doctor.Specialty = updatedDoctor.Specialty ?? doctor.Specialty;
            doctor.License = updatedDoctor.License ?? doctor.License;
            doctor.Description = updatedDoctor.Description ?? doctor.Description;
            doctor.address.City = updatedDoctor.City ??  doctor.address.City;
            doctor.address.State = updatedDoctor.State ?? doctor.address.State;
            doctor.address.ZipCode = updatedDoctor.Zipcode ?? doctor.address.ZipCode;
            doctor.address.Street = updatedDoctor.Street ?? doctor.address.Street;
            doctor.Reviews = updatedDoctor.Reviews ?? doctor.Reviews;
            doctor.Rating = updatedDoctor.Rating;
            doctor.AppointmentFee = updatedDoctor.AppointmentFee;
            doctor.ImageUrl = updatedDoctor.ImageUrl ?? doctor.ImageUrl;
            doctor.AvailableSlots = updatedDoctor.AvailableSlots;
            doctor.RegisteredSlots = updatedDoctor.RegisteredSlots;


            // Save the updated doctor to the database
            await _context.Hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);

            return new
            {
                Success = true,
                Message = "Doctor details updated successfully",
                Doctor = new
                {   
                    doctor.Id,
                    doctor.Name,
                    doctor.Specialty,
                    doctor.License,
                    doctor.Description,
                    doctor.address,
                   
                    doctor.AppointmentFee,
                    doctor.ImageUrl,
                    doctor.AvailableSlots
                }
            };
        }



        public async Task<string> AddAvailableSlot(string doctorId, DateTime slot)
        {
            // Validate the doctorId
            if (!ObjectId.TryParse(doctorId, out _))
                throw new Exception("Invalid doctor ID");

            // Find the hospital containing the doctor
            var hospital = await _context.Hospitals.Find(h => h.Doctors.Any(d => d.Id.ToString() == doctorId)).FirstOrDefaultAsync();
            if (hospital == null)
                throw new Exception("Doctor not found");

            // Find the doctor
            var doctor = hospital.Doctors.FirstOrDefault(d => d.Id.ToString() == doctorId);
            if (doctor == null)
                throw new Exception("Doctor not found");

            // Add the slot if it doesn't conflict with registered slots
            if (!doctor.RegisteredSlots.Contains(slot))
            {
                doctor.AvailableSlots.Add(slot);
                await _context.Hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);
                return "Slot added to available slots";
            }

            throw new Exception("Slot conflicts with registered slots");
        }

        public async Task<string> RegisterSlot(string doctorId, DateTime slot)
        {
            if (!ObjectId.TryParse(doctorId, out _))
                throw new Exception("Invalid doctor ID");

            var hospital = await _context.Hospitals.Find(h => h.Doctors.Any(d => d.Id.ToString() == doctorId)).FirstOrDefaultAsync();
            if (hospital == null)
                throw new Exception("Doctor not found");

            var doctor = hospital.Doctors.FirstOrDefault(d => d.Id.ToString() == doctorId);
            if (doctor == null)
                throw new Exception("Doctor not found");

            if (doctor.AvailableSlots.Contains(slot))
            {
                doctor.AvailableSlots.Remove(slot);
                doctor.RegisteredSlots.Add(slot);
                await _context.Hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);
                return "Slot registered successfully";
            }

            throw new Exception("Slot is not available for registration");
        }




        public async Task<string> DeleteDoctorById(string doctorId)
        {
            // Validate if the provided doctorId is a valid ObjectId
            if (!ObjectId.TryParse(doctorId, out _))
                throw new Exception("Invalid doctor ID");

            // Find the hospital that contains the doctor
            var hospital = await _context.Hospitals.Find(h => h.Doctors.Any(d => d.Id.ToString() == doctorId)).FirstOrDefaultAsync();
            if (hospital == null)
                throw new Exception("Hospital not found");

            // Find the doctor in the hospital's doctor list
            var doctor = hospital.Doctors.FirstOrDefault(d => d.Id.ToString() == doctorId);
            if (doctor == null)
                throw new Exception("Doctor not found");

            // Remove the doctor from the hospital's list of doctors
            hospital.Doctors.Remove(doctor);

            // Save the updated hospital document
            await _context.Hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);

            return "Doctor deleted successfully";
        }

        public async Task<string> CreateEHR(EHR ehr)
        {
            // Check if the patient already has an EHR
            var existingEHR = await _context.EHRs.Find(e => e.PatientId == ehr.PatientId).FirstOrDefaultAsync();
            if (existingEHR != null)
                throw new Exception("EHR already exists for the patient");

            // Insert the new EHR
            await _context.EHRs.InsertOneAsync(ehr);

            var updateDefinition = Builders<Patient>.Update
            .Set(e => e.EHR, ehr);  // Update specific fields


            await _context.Patients
                .UpdateOneAsync(e => e.Id == ObjectId.Parse(ehr.PatientId), updateDefinition);

            return "EHR created successfully";
        }

        public async Task<string> UpdateHospitalById(string hospitalId, Hospital updatedHospital)
        {
            // Validate if the provided hospitalId is a valid ObjectId
            if (!ObjectId.TryParse(hospitalId, out _))
                throw new Exception("Invalid Hospital ID");

            // Ensure the updated Hospital keeps the original ID
            updatedHospital.Id = ObjectId.Parse(hospitalId);

            // Replace the existing Hospital document with the updated one
            var result = await _context.Hospitals.ReplaceOneAsync(
                e => e.Id == ObjectId.Parse(hospitalId),
                updatedHospital
            );

            if (result.MatchedCount == 0)
                throw new Exception("Hospital not found");

            return "Hospital updated successfully.";
        }
        public async Task<EHR> GetEHRById(string ehrId)
        {
            // Validate if the provided ehrId is a valid ObjectId
            if (!ObjectId.TryParse(ehrId, out _))
                throw new Exception("Invalid EHR ID");
            // Find the EHR document by ID
            var ehr = await _context.EHRs.Find(e => e.Id == ObjectId.Parse(ehrId)).FirstOrDefaultAsync();
            if (ehr == null)
                throw new Exception("EHR not found");
            // Ensure the patient's EHR field is set to this EHR document if it's not already
            var patient = await _context.Patients.Find(p => p.Id == ObjectId.Parse(ehr.PatientId)).FirstOrDefaultAsync();
            if (patient != null && patient.EHR == null)
            {
                var updateDefinition = Builders<Patient>.Update.Set(p => p.EHR, ehr);
                await _context.Patients.UpdateOneAsync(p => p.Id == patient.Id, updateDefinition);
            }
            return ehr;
        }
        public async Task<string> UpdateEHRById(string ehrId, EHR updatedEHR)
        {
            // Validate if the provided ehrId is a valid ObjectId
            if (!ObjectId.TryParse(ehrId, out _))
                throw new Exception("Invalid EHR ID");
            // Ensure the updated EHR keeps the original ID
            updatedEHR.Id = ObjectId.Parse(ehrId);
            // Replace the existing EHR document with the updated one
            var result = await _context.EHRs.ReplaceOneAsync(e => e.Id == ObjectId.Parse(ehrId), updatedEHR);
            if (result.MatchedCount == 0)
                throw new Exception("EHR not found");
            // Now, update the patient's EHR field in the Patients collection
            var patient = await _context.Patients.Find(p => p.Id == ObjectId.Parse(updatedEHR.PatientId)).FirstOrDefaultAsync();
            if (patient != null)
            {
                var updateDefinition = Builders<Patient>.Update.Set(p => p.EHR, updatedEHR);
                await _context.Patients.UpdateOneAsync(p => p.Id == patient.Id, updateDefinition);
            }
            return "EHR updated successfully";
        }

        public async Task<HospitalResult> GetHospitalById(string id)
        {
            try
            {
                var objectId = new ObjectId(id); // Convert string ID to MongoDB ObjectId
                var user = await _context.Hospitals.Find(u => u.Id == objectId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new HospitalResult { IsSuccess = false, Message = "hospital not found." };
                }
                return new HospitalResult { IsSuccess = true, Data = user, Message = $"hospital with {id} valid" };
            }
            catch (Exception ex)
            {
                return new HospitalResult { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }


        public class HospitalResult
        {
            public bool IsSuccess { get; set; }
            public Hospital? Data { get; set; }
            public string? Role { get; set; }
            public required string Message { get; set; }
        }

   



    }

}
