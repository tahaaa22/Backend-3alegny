
using _3alegny.RepoLayer;

namespace _3alegny.Service_layer
{
    public class PharmacyLogic
    {
        private readonly MongoDbContext _context;

        public PharmacyLogic(MongoDbContext context)
        {
            _context = context;
        }
    }
}
