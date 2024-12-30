
using _3alegny.Entities;
using _3alegny.RepoLayer;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace _3alegny.Service_layer
{
    public class PharmacyLogic
    {
        private readonly MongoDbContext _context;

        public PharmacyLogic(MongoDbContext context)
        {
            _context = context;
        }
        //Get pharmacy by id
        public async Task<PharmacyResult<Pharmacy>> GetPharmacyById(string id)
        {
            try
            {
                var objectId = new ObjectId(id); // Convert string ID to MongoDB ObjectId
                var user = await _context.Pharmacies.Find(u => u.Id == objectId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new PharmacyResult<Pharmacy> { IsSuccess = false, Message = "hospital not found." };
                }
                return new PharmacyResult<Pharmacy> { IsSuccess = true, Data = user, Message = $"hospital with {id} valid" };
            }
            catch (Exception ex)
            {
                return new PharmacyResult<Pharmacy> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        //ADD a new drug to the pharmacy
        public async Task<DrugsResult> AddDrug(string pharmacyId, Drugs drug)
        {
            try
            {
                var newdrug = new Drugs
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = drug.Name,
                    Quantity = drug.Quantity,
                    Price = drug.Price
                };
                // Check if the drug already exists using drug name
                var drugcheck = await _context.Drugs.Find(d => d.Name == drug.Name).FirstOrDefaultAsync();
                // If drug name exists, update the quantity
                if (drugcheck == null)
                {
                    await _context.Drugs.InsertOneAsync(newdrug);
                }
                else
                {
                    return new DrugsResult { IsSuccess = false,Message = "Drug already added" };
                }
                var pharmacy = await _context.Pharmacies.Find(p => p.Id == ObjectId.Parse(pharmacyId)).FirstOrDefaultAsync();
                if (pharmacy == null)
                {
                    return new DrugsResult { IsSuccess = false, Message = "Pharmacy not found." };
                }
                pharmacy.Drugs.Add(newdrug);
                await _context.Pharmacies.ReplaceOneAsync(p => p.Id == ObjectId.Parse(pharmacyId), pharmacy);
                return new DrugsResult { IsSuccess = true, Data = newdrug, Message = "Drug added successfully" };
            }
            catch (Exception ex)
            {
                return new DrugsResult { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        //Update Drug Quantity in the pharmacy
        public async Task<DrugsResult> UpdateDrugQuantity(string pharmacyId, string drugName, int quantityToAdd, bool flag)
        {
            
            try
            {
                
                var drug = await _context.Drugs.Find(d => d.Name == drugName).FirstOrDefaultAsync();
                if (drug == null)
                {
                    return new DrugsResult
                    {
                        IsSuccess = false,
                        Message = "Drug not found in the total inventory."
                    };
                }

                // Find the pharmacy and the drug in its stock
                var pharmacy = await _context.Pharmacies
                    .Find(p => p.Id == ObjectId.Parse(pharmacyId))
                    .FirstOrDefaultAsync();

                if (pharmacy == null)
                {
                    return new DrugsResult
                    {
                        IsSuccess = false,
                        Message = "Pharmacy not found."
                    };
                }

                var pharmacyDrug = pharmacy.Drugs.FirstOrDefault(d => d.Id == drug.Id);

                if (pharmacyDrug == null)
                {
                    // If the drug is not in the pharmacy, add it to the pharmacy's drug list
                    pharmacy.Drugs.Add(new Drugs
                    {
                        Id = drug.Id,
                        Name = drugName,
                        Quantity = quantityToAdd,
                        Price = drug.Price 
                    });
                }
                else
                {
                    // If the drug exists in the pharmacy, update its quantity
                    if (flag)
                    {
                        pharmacyDrug.Quantity += quantityToAdd;
                    }
                    else
                    {
                        pharmacyDrug.Quantity -= quantityToAdd;
                    }
                }

                if (flag)
                {
                    drug.Quantity += quantityToAdd;
                }
                else
                {
                    drug.Quantity -= quantityToAdd;
                }
                await _context.Drugs.ReplaceOneAsync(d => d.Id == drug.Id, drug);

                
                await _context.Pharmacies.ReplaceOneAsync(p => p.Id == ObjectId.Parse(pharmacyId), pharmacy);

                return new DrugsResult
                {
                    IsSuccess = true,
                    Data = drug,
                    Message = "Drug quantity updated successfully in the pharmacy and inventory."
                };
            }
            catch (Exception ex)
            {
                
                return new DrugsResult
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }



        //GET total number of drugs by category from Drugs collection
        public async Task<PharmacyResult<List<dynamic>>> GetAllDrugs()
        {
            try
            {
                var projection = Builders<Drugs>.Projection
                    .Include(d => d.Name)
                    .Include(d => d.Quantity)
                    .Exclude("_id");

                var drugs = await _context.Drugs
                    .Find(_ => true)
                    .Project<dynamic>(projection)
                    .ToListAsync();

                return new PharmacyResult<List<dynamic>>
                {
                    IsSuccess = true,
                    Data = drugs,
                    Message = "All drugs returned with their quantities"
                };
            }
            catch (Exception ex)
            {
                return new PharmacyResult<List<dynamic>>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }


        public class PharmacyResult<T>
        {
            public bool IsSuccess { get; set; }
            public T? Data { get; set; }
            public string? role { get; set; }
            public required string Message { get; set; }
        }

        public class DrugsResult
        {
            public bool IsSuccess { get; set; }
            public Drugs? Data { get; set; }
            public required string Message { get; set; }
        }
    }
}
