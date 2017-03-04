namespace MVCMongo.Core.Abstraction
{
    using MVCMongo.Core.Model;
    using System.Collections.Generic;

    public interface IProductRepostory
    {
        IEnumerable<Product> GetProducts();

        Product GetProduct(int id);

        Product Create(Product p);

        void Update(int id, Product p);

        void Remove(int id);
    }
}
