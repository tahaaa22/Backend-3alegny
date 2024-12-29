using _3alegny.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;


public class EHR
{

    [BsonId] // Explicitly set the _id field
    public ObjectId Id { get; set; }  // MongoDB will handle the _id field

    [BsonRepresentation(BsonType.ObjectId)]
    public string PatientId { get; set; }
    // Patient Demographics
    public string PatientName { get; set; }
    public int PatientAge { get; set; }
    public string PatientGender { get; set; }
    public string PatientContactDetails { get; set; }
    public string PatientInsuranceInfo { get; set; }
    public string MedicalHistory { get; set; }
    // Clinical Data
    public List<string> Diagnoses { get; set; } = new();
    public List<string> Medications { get; set; } = new();
    public List<string> Allergies { get; set; } = new();
    public List<string> LabResults { get; set; } = new();
    public List<string> Immunizations { get; set; } = new();
    public Dictionary<string, string> VitalSigns { get; set; } = new();
    public string ClinicalNotes { get; set; }
    // Medical Imaging
    public List<string> ImagingLinks { get; set; } = new();
    // Treatment Plans
    public string TreatmentPlans { get; set; }
    // Procedures
    public string ProcedureHistory { get; set; }
    // Care Coordination
    public string CareCoordinationInfo { get; set; }
    // Financial Information
    public string FinancialInfo { get; set; }
    // Consent and Legal Documents
    public List<string> LegalDocuments { get; set; } = new();
    // Patient Portal Access
    public string PatientPortalAccess { get; set; }
    // Reporting and Analytics
    public string ReportingAndAnalytics { get; set; }
}

