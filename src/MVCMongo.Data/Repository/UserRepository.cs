namespace MVCMongo.Data.Repository
{
    using MVCMongo.Core.Abstraction;
    using System.Linq;
    using MVCMongo.Core.Model;
    using MongoDB.Driver;

    public class UserRepository : IUserRepository
    {
        private readonly IMongoContext _context = null;
        public UserRepository(IMongoContext context)
        {
            _context = context;
        }
        public User GetUserByName(string userName)
        {
            return _context.Users.Find(x => x.UserName == userName).FirstOrDefault();
        }
    }
}
