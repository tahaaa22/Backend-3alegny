using _3alegny.RepoLayer;

namespace _3alegny.Service_layer
{
    public class OrdersLogic
    {
        private readonly MongoDbContext _context;

        public OrdersLogic(MongoDbContext context)
        {
            _context = context;
        }
    }
}
