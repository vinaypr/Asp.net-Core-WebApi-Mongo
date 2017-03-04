namespace MVCMongo.Services
{
    using AutoMapper;
    using MVCMongo.Core.Abstraction;
    using MVCMongo.Core.Model;
    using MVCMongo.Core.ViewModel;
    using System.Collections.Generic;

    public class ProductService : IProductService
    {
        IProductRepostory _productRepository;
        IMapper _mapper;
        public ProductService(IProductRepostory productRepository, IMapper mapper) {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public IEnumerable<ProductViewModel> GetProducts()
        {
            var products = _productRepository.GetProducts();
            return _mapper.Map<List<ProductViewModel>>(products);
        }

        public ProductViewModel GetProduct(int id)
        {
            var product = _productRepository.GetProduct(id);
            return _mapper.Map<ProductViewModel>(product);
        }

        public ProductViewModel Create(ProductViewModel p)
        {
            var product = _mapper.Map<Product>(p);
            var newProduct = _productRepository.Create(product);
            return _mapper.Map<ProductViewModel>(product);
        }

        public void Update(int id, ProductViewModel p)
        {
            var product = _mapper.Map<Product>(p);
            _productRepository.Update(id, product);
        }
        public void Remove(int id)
        {
            _productRepository.Remove(id);
        }
    }
}
