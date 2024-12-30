using _3alegny.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;


public class EHR
{

    [BsonId] // Explicitly set the _id field
    public ObjectId Id { get; set; }  // MongoDB will handle the _id field

    [BsonRepresentation(BsonType.ObjectId)]
    public string PatientId { get; set; }
   
    
    public string? PatientName { get; set; }
    public DateTime Birthdate  { get; set; } 
    public string? PatientGender { get; set; }



    /// ////////ADD////////////

    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }


    public double? Temperature { get; set; } // e.g., 36.5
    public double? Weight { get; set; }      // e.g., 70.5 kg
    
    public string? BloodPressure { get; set; } // e.g., "120/80 mmHg"
    public int? HeartRate { get; set; }      // e.g., 75 BPM

    //Statistics

    public List <double>? TemperatureStatics { get; set; } 
    public List <double>? WeightStatics { get; set; }    

    public List <string>? BloodPressureStatics { get; set; } 
    public List <int>? HeartRateStatics { get; set; }      

    public string? PatientInsuranceInfo { get; set; }
    public string? MedicalHistory { get; set; }
    // Clinical Data
    public List<string>? Diagnoses { get; set; } = new();
    public List<string>? Medications { get; set; } = new();
    public List<string>? Allergies { get; set; } = new();
    public List<string>? LabResults { get; set; } = new();
    public List<string>? Immunizations { get; set; } = new();
    /*public Dictionary<string, string> VitalSigns { get; set; } = new()*/
   
    // Medical Imaging
    public List<string>? ImagingLinks { get; set; } = new();
    // Treatment Plans
    public string? TreatmentPlans { get; set; } 
    // Procedures
    public List <string>? ProcedureHistory { get; set; }
    // Care Coordination
    public string?  CareCoordinationInfo { get; set; } 
    // Financial Information
    public List <string>? Billing  { get; set; }
  
   
   
}