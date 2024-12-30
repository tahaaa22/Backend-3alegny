using _3alegny.Entities;
using _3alegny.RepoLayer;
using MongoDB.Bson;
using MongoDB.Driver;

namespace _3alegny.Service_layer
{
    public class OrdersLogic
    {
        private readonly MongoDbContext _context;

        public OrdersLogic(MongoDbContext context)
        {
            _context = context;
        }

        // Create new order
        public async Task<OrderResult<string>> CreateOrder(string patientId, string pharmacyId, OrderRequest neworder)
        {
            try
            {
                //Search for patient by patient id and add a new order to the list of orders in the patient
                var pid = ObjectId.Parse(patientId);
                var patient = await _context.Patients.Find(p => p.Id == pid).FirstOrDefaultAsync();
                if (patient == null)
                {
                    return new OrderResult<string> { IsSuccess = false, Message = "Patient not found" };
                }
                var oid = ObjectId.GenerateNewId();
                var order = new Order
                {
                    OrderId = oid,
                    PatientId = patientId,
                    PharmacyId = pharmacyId,
                    //Adding the list of drugs to the order
                    Drugs = neworder.Drugs.Select((drug, index) => new Drugs
                    {
                        Name = drug,
                        Category = neworder.DrugCategories[index],
                        Quantity = neworder.DrugQuantities[index]
                    }).ToList(),
                    //Calculating the total cost of the order
                    TotalCost = (int)neworder.Drugs.Select((drug, index) => neworder.DrugQuantities[index] * _context.Drugs.Find(d => d.Name == drug).FirstOrDefault().Price).Sum(),
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    // Adding the address to the order
                    OrderAddress = new Address
                    {
                        City = neworder.city,
                        Street = neworder.street,
                        State = neworder.state,
                        ZipCode = neworder.zipcode

                    }


                };
                //Add the order to the orders collection and patient collection
                await _context.Orders.InsertOneAsync(order);
                patient.Orders.Add(order);
                await _context.Patients.ReplaceOneAsync(h => h.Id == patient.Id, patient);
                return new OrderResult<string> { IsSuccess = true, Message = "Order created successfully" };
            }
            catch (Exception ex)
            {
                return new OrderResult<string> { IsSuccess = false, Message = ex.Message };
            }
        }
        // Get patient orders
        public async Task<OrderResult<List<Order>>> GetPatientOrders(string patientId)
        {
            try
            {
                var pid = ObjectId.Parse(patientId);
                var patient = await _context.Patients.Find(p => p.Id == pid).FirstOrDefaultAsync();
                if (patient == null)
                {
                    return new OrderResult<List<Order>> { IsSuccess = false, Message = "Patient not found" };
                }
                var orders = await _context.Orders.Find(o => o.PatientId == patientId).ToListAsync();
                return new OrderResult<List<Order>> { IsSuccess = true, Data = orders };
            }
            catch (Exception ex)
            {
                return new OrderResult<List<Order>> { IsSuccess = false, Message = ex.Message };
            }
        }

        //Get all orders by pharmacy id
        public async Task<OrderResult<List<Order>>> GetPharmacyOrders(string pharmacyId)
        {
            try
            {
                var orders = await _context.Orders.Find(o => o.PharmacyId == pharmacyId).ToListAsync();
                return new OrderResult<List<Order>> { IsSuccess = true, Data = orders };

            }
            catch (Exception ex) 
            {
                return new OrderResult<List<Order>> { IsSuccess = false, Message = ex.Message };
            }
        }

        //Cancel Order
        public async Task<OrderResult<string>> CancelOrder(string pid,string oid)
        {
            try
            {
                var patientId = ObjectId.Parse(pid);
                var patient = await _context.Patients.Find(p => p.Id == patientId).FirstOrDefaultAsync();
                if (patient == null)
                {
                    return new OrderResult<string> { IsSuccess = false, Message = "Patient not found" };
                }
                var orders = await _context.Orders.Find(o => o.PatientId == pid).ToListAsync();
                if (orders.Count == 0)
                {
                    return new OrderResult<string> { IsSuccess = false, Message = "No orders found" };
                }
                //Delete order by order id in patient orders list
                foreach (var order in orders)
                {
                    if (order.OrderId == ObjectId.Parse(oid))
                        patient.Orders.Remove(order);
                        await _context.Orders.DeleteOneAsync(o => o.OrderId == order.OrderId);
                }
                return new OrderResult<string> { IsSuccess = true, Message = "Orders cancelled successfully" };
            }
            catch (Exception ex)
            {
                return new OrderResult<string> { IsSuccess = false, Message = ex.Message };
            }
        }

        //Get all orders
        public async Task<OrderResult<List<Order>>> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders.Find(_ => true).ToListAsync();
                return new OrderResult<List<Order>> { IsSuccess = true, Data = orders };
            }
            catch (Exception ex)
            {
                return new OrderResult<List<Order>> { IsSuccess = false, Message = ex.Message };
            }
        }
    }
}
public class OrderResult<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}
