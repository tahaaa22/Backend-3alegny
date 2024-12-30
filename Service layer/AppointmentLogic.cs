using _3alegny.Entities;
using _3alegny.RepoLayer;
using MongoDB.Bson;
using MongoDB.Driver;
using static AppointmentEndpoints;


namespace _3alegny.Service_layer
{
    public class AppointmentLogic
    {
        private readonly MongoDbContext _context;

        public AppointmentLogic(MongoDbContext context)
        {
            _context = context;
        }

        // Schedule an appointment
        public async Task<AppointmentResponse<string>> ScheduleAppointment(string pid, AppointmentRequest Appointment)
        {
            try
            {
                var patientid = ObjectId.Parse(pid);
                var patient = await _context.Patients.Find(p => p.Id == patientid).FirstOrDefaultAsync();
                var appointment = new Appointments
                {
                    AppointmentDate = Appointment.Date,
                    AppointmentTime = Appointment.Time,
                    Status = "Scheduled",
                    DoctorName = Appointment.DoctorName,
                    Department = Appointment.Department,
                    PHR = patient.PHR,
                    PatientId = pid,
                    HospitalName = Appointment.HospitalName
                };

                await _context.Appointments.InsertOneAsync(appointment);
                // Add the new appointment to the patient's Appointments list
                var update = Builders<Patient>.Update.Push(p => p.Appointments, appointment);
                await _context.Patients.UpdateOneAsync(p => p.Id == patientid, update);

                return new AppointmentResponse<string> { IsSuccess = true, Message = "Appointement Schedueled Successfully" }; ;
            }
            catch (Exception ex)
            {
                return new AppointmentResponse<string> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }

        }

        // Get a patient's appointments using patient ID
        public async Task<List<Appointments>> GetAppointments(string pid)
        {
            try
            {
                var patientid = ObjectId.Parse(pid);
                var patient = await _context.Patients.Find(p => p.Id == patientid).FirstOrDefaultAsync();
                return patient.Appointments;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Get all appointments
        public async Task<List<Appointments>> GetAllAppointments()
        {
            try
            {
                return await _context.Appointments.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Get all appointments for a specific doctor
        public async Task<List<Appointments>> GetDoctorAppointments(string doctorName)
        {
            try
            {
                return await _context.Appointments.Find(a => a.DoctorName == doctorName).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Get all appointments for a specific hospital
        public async Task<List<Appointments>> GetHospitalAppointments(string hospitalName)
        {
            try
            {
                return await _context.Appointments.Find(a => a.HospitalName == hospitalName).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Get all appointments for a specific department
        public async Task<List<Appointments>> GetDepartmentAppointments(string department)
        {
            try
            {
                return await _context.Appointments.Find(a => a.Department == department).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Change an appointment
        public async Task<AppointmentResponse<string>> ChangeAppointment(string pid, string time, AppointmentRequest appointmentRequest)
        {
            try
            {
                var patientId = ObjectId.Parse(pid);
                var patient = await _context.Patients.Find(p => p.Id == patientId).FirstOrDefaultAsync();
                if (patient == null)
                {
                    return new AppointmentResponse<string> { IsSuccess = false, Message = "Patient not found" };
                }

                var existingAppointment = patient.Appointments.FirstOrDefault(a => a.AppointmentTime == time);
                if (existingAppointment == null)
                {
                    return new AppointmentResponse<string> { IsSuccess = false, Message = "No appointment found with the specified time" };
                }

                // Update the existing appointment with new details
                existingAppointment.AppointmentDate = appointmentRequest.Date;
                existingAppointment.DoctorName = appointmentRequest.DoctorName;
                existingAppointment.Department = appointmentRequest.Department;
                existingAppointment.HospitalName = appointmentRequest.HospitalName;

                // Update the patient's appointments list in the database
                var update = Builders<Patient>.Update.Set(p => p.Appointments[-1], existingAppointment);
                await _context.Patients.UpdateOneAsync(p => p.Id == patientId && p.Appointments.Any(a => a.AppointmentTime == time), update);

                return new AppointmentResponse<string> { IsSuccess = true, Message = "Appointment changed successfully" };
            }
            catch (Exception ex)
            {
                return new AppointmentResponse<string> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        // Cancel an appointment
        public async Task<AppointmentResponse<string>> CancelAppointment(string pid, string time)
        {
            try
            {
                var patientId = ObjectId.Parse(pid);
                var patient = await _context.Patients.Find(p => p.Id == patientId).FirstOrDefaultAsync();
                if (patient == null)
                {
                    return new AppointmentResponse<string> { IsSuccess = false, Message = "Patient not found" };
                }

                var existingAppointment = patient.Appointments.FirstOrDefault(a => a.AppointmentTime == time);
                if (existingAppointment == null)
                {
                    return new AppointmentResponse<string> { IsSuccess = false, Message = "No appointment found with the specified time" };
                }

                // Remove the appointment from the patient's appointments list
                var update = Builders<Patient>.Update.PullFilter(p => p.Appointments, a => a.AppointmentTime == time);
                await _context.Patients.UpdateOneAsync(p => p.Id == patientId, update);

                // Remove the appointment from the database
                await _context.Appointments.DeleteOneAsync(a => a.PatientId == pid && a.AppointmentTime == time);

                return new AppointmentResponse<string> { IsSuccess = false, Message = "Appointment cancelled successfully" };
            }
            catch (Exception ex)
            {
                return new AppointmentResponse<string> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }


    }
}

    public class AppointmentResponse<T>
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
    }
