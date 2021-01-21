using Microsoft.AspNetCore.Mvc;
using Moq;
using SampleUnitTest.Web.Controllers;
using SampleUnitTest.Web.Helpers;
using SampleUnitTest.Web.Models;
using SampleUnitTest.Web.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SampleUnitTest.Test
{
    public class ProductApiControllerTest
    {
        private Mock<IRepository<Product>> _mockRepository;
        private ProductApiController _productApiController;
        private Helper _helper;
        private List<Product> products;
        public ProductApiControllerTest()
        {
            _mockRepository = new Mock<IRepository<Product>>();
            _productApiController = new ProductApiController(_mockRepository.Object);
            _helper = new Helper();

            products = new List<Product>() { new Product { Id = 1,Name = "Kalem",Price = 100, Stock = 50, Color = "Kırmızı" },
                                                new Product{Id = 2,Name = "Defter",Price = 200,Stock = 500,Color = "Mavi" }};
        }

        [Theory]
        [InlineData(4,5,9)]
        public void Add_SampleValues_ReturnTotal(int a, int b, int total)
        {
            var result = _helper.add(a, b);
            Assert.Equal(total,result);
        }

        [Fact]
        public async void GetProduct_ActionExecutes_ReturnOkResultWithProduct()
        {
            _mockRepository.Setup(x => x.GetAll()).ReturnsAsync(products);

            var result = await _productApiController.GetProduct();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(2, returnProducts.ToList().Count);
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepository.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _productApiController.GetProduct(productId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            var product = GetProductModel(productId);
            _mockRepository.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _productApiController.GetProduct(productId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(productId, returnProduct.Id);
            Assert.Equal(product.Name, returnProduct.Name);

        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_IsIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = GetProductModel(productId);
            var result = _productApiController.PutProduct(2, product);
            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = GetProductModel(productId);
            _mockRepository.Setup(x => x.Update(product));

            var result = _productApiController.PutProduct(productId, product);
            _mockRepository.Verify(x => x.Update(product), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void PostProduct_ActionExecutes_ReturnCreatedAction()
        {
            var product = products.FirstOrDefault();
            _mockRepository.Setup(x => x.Create(product)).Returns(Task.CompletedTask);

            var result = await _productApiController.PostProduct(product);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            _mockRepository.Verify(x => x.Create(product), Times.Once);

            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepository.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var resultNotFound = await _productApiController.DeleteProduct(productId);
            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = GetProductModel(productId);
            _mockRepository.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            _mockRepository.Setup(x => x.Delete(product));

            var noContentResult = await _productApiController.DeleteProduct(productId);
            _mockRepository.Verify(x => x.Delete(product), Times.Once);

            Assert.IsType<NoContentResult>(noContentResult.Result);
        }

        private Product GetProductModel(int productId)
        {
            return products.First(x => x.Id == productId);
        }
    }
}
