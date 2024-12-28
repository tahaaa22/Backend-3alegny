using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _3alegny.Entities
{
    using MongoDB.Bson.Serialization.Attributes;

    public enum UserRole
    {
        [BsonRepresentation(BsonType.String)]
        Admin,

        [BsonRepresentation(BsonType.String)]
        Patient,

        [BsonRepresentation(BsonType.String)]
        Hospital,

        [BsonRepresentation(BsonType.String)]
        Pharmacy
    }

    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(Admin), typeof(Patient), typeof(Hospital), typeof(Pharmacy))]
    public class User
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public UserRole Role { get; set; } = UserRole.Patient;
        public string? Password { get; set; }
        public ContactInfo? contactInfo { get; set; }
        public Address? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }

    // Subclass: Admin
    public class Admin : User
    {
        public List<EHR> EMRs { get; set; } = new List<EHR>();
        public List<PHR> PHRs { get; set; } = new List<PHR>();
        public List<Hospital> Hospitals { get; set; } = new List<Hospital>();
        public List<Pharmacy> Pharmacies { get; set; } = new List<Pharmacy>();
    }

    // Subclass: Patient
    public class Patient : User
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Appointments> Appointments { get; set; } = new List<Appointments>();
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public List<Insurance>? Insurance { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public List<EHR> EHR { get; set; } = new List<EHR>();




    }

    // Subclass: Hospital
    public class Hospital : User
    {
        public List<Department> Departments { get; set; } = new List<Department>();
        public List<EHR> EHRs { get; set; } = new List<EHR>();
        public List<Appointments> Appointments { get; set; } = new List<Appointments>();
        public Double? Rating { get; set; } = 0.0;
        public List<Doctors> Doctors { get; set; } = new List<Doctors>();
        public List<Insurance> InsuranceAccepted { get; set; } = new List<Insurance>();
    }

    // Subclass: Pharmacy
    // Subclass: Pharmacy
    public class Pharmacy : User
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Drugs> Drugs { get; set; } = new List<Drugs>();
        public Double Rating { get; set; } = 0.0;
    }
}
