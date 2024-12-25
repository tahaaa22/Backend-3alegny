using _3alegny.Entities;
using _3alegny.RepoLayer;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

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

        // Method to add a doctor to a hospital
        public async Task<string> AddDoctor(Doctors doctor)

        {   // Ensure the doctor's ID is generated
            if (string.IsNullOrEmpty(doctor.Id) || !ObjectId.TryParse(doctor.Id, out _))
            {
                doctor.Id = ObjectId.GenerateNewId().ToString();
            }


            //Check if the hospital exists
            var hospital = await _context.Hospitals.Find(h => h.Id.ToString() == doctor.HospitalId).FirstOrDefaultAsync();
            if (hospital == null)
                throw new Exception("Hospital not found");

            

            // Check if the doctor's specialty matches an existing department in the hospital
            var matchingDepartment = hospital.Departments.FirstOrDefault(dept => dept.Name.Equals(doctor.Specialty, StringComparison.OrdinalIgnoreCase));
            if (matchingDepartment == null)
                throw new Exception("Department not found");


            hospital.Doctors.Add(doctor);
            await _context.Hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);

            return "Doctor added successfully";
        }

        public async Task<string> UpdateDoctorById(string doctorId, Doctors updatedDoctor)  //need more enhacements
        {
            // Validate if the provided doctorId is a valid ObjectId
            if (!ObjectId.TryParse(doctorId, out _))
                throw new Exception("Invalid doctor ID");

            // Find the hospital that contains the doctor
            var hospital = await _context.Hospitals.Find(h => h.Doctors.Any(d => d.Id == doctorId)).FirstOrDefaultAsync();
            if (hospital == null)
                throw new Exception("Doctor not found");

            // Find the index of the doctor in the list
            var doctorIndex = hospital.Doctors.FindIndex(d => d.Id == doctorId);
            if (doctorIndex == -1)
                throw new Exception("Doctor not found");

            // If the doctor exists, we update their details
            updatedDoctor.Id = doctorId;  // Keep the original doctor ID
            hospital.Doctors[doctorIndex] = updatedDoctor;

            // Save the updated hospital document
            await _context.Hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);

            return "Doctor details retrieved and updated successfully";
        }


        public async Task<string> DeleteDoctorById(string doctorId)
        {
            // Validate if the provided doctorId is a valid ObjectId
            if (!ObjectId.TryParse(doctorId, out _))
                throw new Exception("Invalid doctor ID");

            // Find the hospital that contains the doctor
            var hospital = await _context.Hospitals.Find(h => h.Doctors.Any(d => d.Id == doctorId)).FirstOrDefaultAsync();
            if (hospital == null)
                throw new Exception("Hospital not found");

            // Find the doctor in the hospital's doctor list
            var doctor = hospital.Doctors.FirstOrDefault(d => d.Id == doctorId);
            if (doctor == null)
                throw new Exception("Doctor not found");

            // Remove the doctor from the hospital's list of doctors
            hospital.Doctors.Remove(doctor);

            // Save the updated hospital document
            await _context.Hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);

            return "Doctor deleted successfully";
        }

    }
}
