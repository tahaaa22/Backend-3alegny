using _3alegny.Entities;
using _3alegny.RepoLayer;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

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
        // Create new order
        public async Task<OrderResult<Order>> CreateOrder(string patientId, string pharmacyId, OrderRequest neworder)
        {
            try
            {
                //Search for patient by patient id and add a new order to the list of orders in the patient
                var pid = ObjectId.Parse(patientId);
                var patient = await _context.Patients.Find(p => p.Id == pid).FirstOrDefaultAsync();
                if (patient == null)
                {
                    return new OrderResult<Order> { IsSuccess = false, Message = "Patient not found" };
                }
                var oid = ObjectId.GenerateNewId();
                var order = new Order
                {
                    Id = oid,
                    PatientId = patientId,
                    PharmacyId = pharmacyId,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    TotalCost = neworder.DrugQuantity * ((int)_context.Drugs.Find(d => d.Name == neworder.Drug).FirstOrDefault().Price),
                    OrderAddress = new Address
                    {
                        City = neworder.city,
                        Street = neworder.street,
                        State = neworder.state,
                        ZipCode = neworder.zipcode,
                    }
                };
                var drug = new Drugs { Id = ObjectId.GenerateNewId(), Name = neworder.Drug, Category = neworder.DrugCategory, Quantity = neworder.DrugQuantity };
                order.Drugs.Add(drug);

                var pharmacy = await _context.Pharmacies.Find(p => p.Id.ToString() == pharmacyId).FirstOrDefaultAsync();
                if (pharmacy == null)
                {
                    return new OrderResult<Order> { IsSuccess = false, Message = "Pharmacy not found" };
                }
                order.PharmacyName = pharmacy.Name;
                //Add the order to the orders collection and patient collection
                await _context.Orders.InsertOneAsync(order);
                patient.Orders.Add(order);

                pharmacy.Orders.Add(order);
                await _context.Pharmacies.ReplaceOneAsync(p => p.Id.ToString() == pharmacyId, pharmacy);

                await _context.Patients.ReplaceOneAsync(h => h.Id == patient.Id, patient);
                return new OrderResult<Order> { IsSuccess = true, Data = order, Message = "Order created successfully" };
            }
            catch (Exception ex)
            {
                return new OrderResult<Order> { IsSuccess = false, Message = ex.Message };
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

        //  Update order status and if order status is accepted we remove the quntity of the drugs in the order from the drugs collection
        public async Task<OrderResult<string>> UpdateOrderStatus(string patientId, string status, string oid)
        {
            try
            {
                var patientID = ObjectId.Parse(patientId);
                var patient = await _context.Patients.Find(p => p.Id == patientID).FirstOrDefaultAsync();
                if (patient == null)
                {
                    return new OrderResult<string> { IsSuccess = false, Message = "Patient not found" };
                }
                var orders = await _context.Orders.Find(o => o.PatientId == patientId).ToListAsync();
                if (orders.Count == 0)
                {
                    return new OrderResult<string> { IsSuccess = false, Message = "No orders found" };
                }
                // Update order status
                foreach (var order in orders)
                {
                    if (order.Id == ObjectId.Parse(oid) && order.Status != status)
                    {
                        order.Status = status;
                        await _context.Orders.ReplaceOneAsync(o => o.Id == order.Id, order);
                        if (status == "Accept")
                        {
                            // Remove the quantity of the drugs in the order from the drugs collection
                            foreach (var drug in order.Drugs)
                            {
                                var drugInStock = await _context.Drugs.Find(d => d.Name == drug.Name).FirstOrDefaultAsync();
                                drugInStock.Quantity -= drug.Quantity;
                                await _context.Drugs.ReplaceOneAsync(d => d.Name == drug.Name, drugInStock);
                            }
                            return new OrderResult<string> { IsSuccess = true, Message = "Order status updated successfully, order accepted!" };
                        }
                        else if (status == "Reject")
                        {
                            patient.Orders.Remove(order);
                            return new OrderResult<string> { IsSuccess = true, Message = "Order status updated, order canceled" };
                        }
                        else
                        {
                            return new OrderResult<string> { IsSuccess = false, Message = "Invalid status" };
                        }
                    }
                }
                return new OrderResult<string> { IsSuccess = false, Message = "Order not found or status unchanged" };
            }
            catch (Exception ex)
            {
                return new OrderResult<string> { IsSuccess = false, Message = ex.Message };
            }
        }




        //Get all orders by pharmacy id
        public async Task<getorderid> GetPharmacyOrders(string pharmacyId)
        {
            try
            {
                //Get all orders by pharmacy id AND list of order ids
                var orders = await _context.Orders.Find(o => o.PharmacyId == pharmacyId).ToListAsync();
                if (orders.Count == 0)
                {
                    return new getorderid { IsSuccess = false, Message = "No orders found" };
                }
                else {
                    List<string> orderid = new List<string>();
                    foreach (var order in orders)
                    {
                        orderid.Add(order.Id.ToString());
                    }
                    return new getorderid { IsSuccess = true, OrderId = orderid, Orders = orders };
                }

            }
            catch (Exception ex) 
            {
                return new getorderid { IsSuccess = false, Message = ex.Message };
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
                    if (order.Id == ObjectId.Parse(oid))
                        patient.Orders.Remove(order);
                        await _context.Orders.DeleteOneAsync(o => o.Id == order.Id);
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

public class getorderid
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public List<string> OrderId { get; set; }
    public List<Order> Orders { get; set; }
}