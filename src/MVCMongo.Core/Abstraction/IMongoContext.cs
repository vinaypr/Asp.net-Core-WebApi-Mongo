namespace MVCMongo.Core.Abstraction
{
    using MongoDB.Driver;
    using MVCMongo.Core.Model;

    public interface IMongoContext
    {
        IMongoCollection<Product> Products { get; }

        IMongoCollection<User> Users { get; }
    }
}
