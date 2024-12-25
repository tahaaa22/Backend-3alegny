using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace _3alegny.Entities
{
    public class EHR
    {
        public ObjectId EHR_id { get; set; }
        public int PatientId { get; set; }
        public string Description { get; set; }
        public string AdminId { get; set; }
        public Admin Admin { get; set; }
        public string HospitalId { get; set; }
        public Hospital Hospital { get; set; }
        public string? Diagnosis { get; set; }
        public Doctors Doctor { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
        [JsonProperty("LabResults")]
        public List<string> LabResults { get; set; } = [];
        public string? ImagingResults { get; set; }
        public string? ProcedureHistory { get; set; }
        public string? MedicationHistory { get; set; }
        public string? Followup { get; set; } = null;

    }
    
}
