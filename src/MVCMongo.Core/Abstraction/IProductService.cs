namespace MVCMongo.Core.Abstraction
{
    using MVCMongo.Core.ViewModel;
    using System.Collections.Generic;

    public interface IProductService
    {
        IEnumerable<ProductViewModel> GetProducts();

        ProductViewModel GetProduct(int id);

        ProductViewModel Create(ProductViewModel p);

        void Update(int id, ProductViewModel p);

        void Remove(int id);
    }
}
