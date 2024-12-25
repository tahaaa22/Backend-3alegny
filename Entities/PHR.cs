using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _3alegny.Entities
{
    public class PHR
    {
        public ObjectId PHRId { get; set; }
        public string PatientId { get; set; }
        public string Notes { get; set; }
        public string AdminId { get; set; }
        public Admin Admin { get; set; }
        public Patient Patient { get; set; }
        public string? MedicalCondition { get; set; }
        public string? Medications { get; set; }
        public string? Allergies { get; set; }
        //public List<Drugs> Drugs { get; set; } = new List<Drugs>(); FIXME: check if this needed here our not
        public string? Surgeries { get; set; }
        public string? FamilyHistory { get; set; }
        public string? SocialHistory { get; set; }
        public string? Immunizations { get; set; }
        public string? VitalSigns { get; set; }
        public string? LabResults { get; set; }
        public string? ImagingResults { get; set; }
        public string ChronicIllness { get; set; }
        public string Diagnosis { get; set; }
        public string Medication { get; set; }
        public string MedicalProcedures { get; set; }
        public string PrescriptionHistory { get; set; }
    }
}
