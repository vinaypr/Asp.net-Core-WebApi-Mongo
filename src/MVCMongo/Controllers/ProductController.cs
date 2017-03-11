namespace MVCMongo.Controllers.Products
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using MVCMongo.Core.Abstraction;
    using MVCMongo.Core.ViewModel;

    public class ProductController : Controller
    {
        private IProductService _productService;

        public ProductController(IProductService productService) {
            _productService = productService;
        }
        // GET: api/products
        [HttpGet]
        [Route("api/Products")]

        public IEnumerable<ProductViewModel> Get()
        {
            return _productService.GetProducts();
        }

        // GET api/products/5
        [HttpGet]
        [Route("api/Products/{id}")]
        public IActionResult Get(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null)
            {
                return new ContentResult { Content = "not found" };
            }
            return new ObjectResult(product);
        }

        [HttpPost]
        [Route("api/Products")]
        public IActionResult Post([FromBody]ProductViewModel p)
        {
            _productService.Create(p);
            return new OkObjectResult(p);
        }

        [HttpPut]
        [Route("api/Products/{id}")]
        public IActionResult Put(int id, [FromBody]ProductViewModel p)
        {
            var product = _productService.GetProduct(id);
            if (product == null)
            {
                return new ContentResult { Content = "not found" };
            }

            _productService.Update(id, p);
            return new OkResult();
        }

        [HttpDelete]
        [Route("api/Products/{id}")]
        public IActionResult Delete(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null)
            {
                return new ContentResult { Content = "not found" };
            }

            _productService.Remove(product.ProductId);
            return new OkResult();
        }
    }
}
