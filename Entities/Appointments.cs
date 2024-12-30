using MongoDB.Bson;

namespace _3alegny.Entities
{
    public class Appointments
    {
        public ObjectId Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string Status { get; set; }
        public string DoctorName { get; set; }
        public string Department { get; set; }
        public PHR PHR { get; set; }
        public string PatientId { get; set; }
        public string HospitalName { get; set; }
    }
}
