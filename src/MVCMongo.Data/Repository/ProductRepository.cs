namespace MVCMongo.Data.Repository
{
    using MongoDB.Driver;
    using MVCMongo.Core.Abstraction;
    using MVCMongo.Core.Model;
    using System.Collections.Generic;
    using System.Linq;

    public class ProductRepository : IProductRepostory
    {
        private readonly IMongoContext _context = null;

        public ProductRepository(IMongoContext MongoContext) {
            _context = MongoContext;
        }
        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.Find(x=> 1 == 1).ToList();
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Find(x => x.ProductId == id).FirstOrDefault();
        }

        public Product Create(Product p)
        {
            _context.Products.InsertOne(p);
            return p;
        }

        public void Update(int id, Product p)
        {
            var product = _context.Products.Find(x => x.ProductId == id).FirstOrDefault();
            if (product != null)
            {
                var filter = Builders<Product>.Filter.Eq("ProductId", id);
                p.Id = product.Id;
                _context.Products.ReplaceOne(filter, p);
            }
        }
        public void Remove(int id)
        {
            var filter = Builders<Product>.Filter.Eq("ProductId", id);
            var operation = _context.Products.DeleteOne(filter);
        }
    }
}
