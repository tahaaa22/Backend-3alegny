using _3alegny.RepoLayer;

namespace _3alegny.Service_layer
{
    public class AppointmentLogic
    {
        private readonly MongoDbContext _context;

        public AppointmentLogic(MongoDbContext context)
        {
            _context = context;
        }
    }
}
