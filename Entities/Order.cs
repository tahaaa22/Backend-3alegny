
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace _3alegny.Entities
{
    public class Order
    {
        public ObjectId OrderId { get; set; }
        public string PatientId { get; set; }
        public string PharmacyId { get; set; }
        public List<Drugs> Drugs { get; set; } = new List<Drugs>();
        public string Status { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int TotalCost { get; set; }
        public Address OrderAddress { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
