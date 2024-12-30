using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _3alegny.Entities
{
    public class Department
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }

        public int AppointmentFee { get; set; } = 0;
        public List<Doctors> AvaliableDoctors { get; set; } = new List<Doctors>();
    }

    public class Insurance
    {
        public ObjectId Id { get; set; }
        public string providerName { get; set; }
        public string policyType { get; set; }
        public Coverage coverage { get; set; }
    }

    public class Coverage
    {
        public ObjectId Id { get; set; }
        public Boolean inPatient { get; set; }
        public Boolean outPatient { get; set; }
        public Boolean emergency { get; set; }
        public Boolean dental { get; set; }
    }
    public class Drugs
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? Manufacturer { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
    }

    // Helper Class: Address
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    //Helper Class: FollowUp
    public class FollowUp
    {
        public ObjectId Id { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public string Date { get; set; }
        public string Notes { get; set; }
        public string Department { get; set; }
    }

    // Helper Class: ContactInfo
    public class ContactInfo
    {
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    // Helper Class: Doctors
    public class Doctors
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string HospitalId { get; set; } 
        public string? License { get; set; }
        public string? Hospital { get; set; } //neglect in add doctor respose  and in update  response
        public string? Description { get; set; }        
        public Address address { get; set; }
        public string? Reviews { get; set; } //neglect in add doctor response 
        public Double Rating { get; set; } //neglect in response in add doctor response
        public int AppointmentFee { get; set; }
        public string ImageUrl { get; set; } // Add image URL property 
        public List<DateTime> AvailableSlots { get; set; } = new List<DateTime>(); 
        public List<DateTime> RegisteredSlots { get; set; } = new List<DateTime>(); //neglect it in add doctor response 
    }

    public class Billing
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
    }

    public class HospitalBilling : Billing
    {

       
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string DepartmentName { get; set; }

        public int AppointmentFee { get; set; }

        public string InsuranceDetails { get; set; }

    }



}
