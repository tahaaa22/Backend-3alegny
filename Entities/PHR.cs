using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _3alegny.Entities
{
    public class PHR
    {
        
        public ObjectId Id { get; set; }
        public string PatientId { get; set; }
        public string Notes { get; set; }
        public string AdminId { get; set; }
        public Admin Admin { get; set; }
        public Patient Patient { get; set; }
        public string? MedicalCondition { get; set; }
        public string Allergies { get; set; }
        public string ChronicIllness { get; set; }
        public string Diagnosis { get; set; }
        public string Medication { get; set; }
        public string FamilyHistory { get; set; }
        public string ImagingResults { get; set; }
        public string LabResultsURL { get; set; }
        public string MedicalProcedures { get; set; }
        public string PrescriptionHistory { get; set; }
        public List<int> Weight { get; set; } = new List<int>();
        public List<int> Height { get; set; } = new List<int>();
        public List<int> BMI { get; set; } = new List<int>();
    }
}
