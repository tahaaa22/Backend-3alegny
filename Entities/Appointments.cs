using MongoDB.Bson;

namespace _3alegny.Entities
{
    public class Appointments
    {
        public ObjectId Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string PatientId { get; set; }
        public Patient Patient { get; set; }
        public string HospitalId { get; set; }
        public Hospital Hospital { get; set; }
    }
}
