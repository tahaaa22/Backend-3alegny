
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace _3alegny.Entities
{
    public class Order
    {
        public ObjectId Id { get; set; }
        public string PatientId { get; set; }
        public string PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public List<Drugs> Drugs { get; set; } = new List<Drugs>();
        public string Status { get; set; } = string.Empty;
        public int TotalDrugQuantity { get; set; }
        public int TotalCost { get; set; }
        public Address OrderAddress { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
