using Microsoft.AspNetCore.Mvc;
using Moq;
using SampleUnitTest.Web.Controllers;
using SampleUnitTest.Web.Models;
using SampleUnitTest.Web.Repository;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SampleUnitTest.Test
{
    public class ProductControllerTest
    {
        private Mock<IRepository<Product>> _mockRepository;
        private ProductController _productController;
        private List<Product> products;
        public ProductControllerTest()
        {
            _mockRepository = new Mock<IRepository<Product>>();
            _productController = new ProductController(_mockRepository.Object);

            products = new List<Product>() { new Product { Id = 1,Name = "Kalem",Price = 100, Stock = 50, Color = "Kırmızı" },
                                                new Product{Id = 2,Name = "Defter",Price = 200,Stock = 500,Color = "Mavi" }};
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _productController.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepository.Setup(repo => repo.GetAll()).ReturnsAsync(products);

            var result = await _productController.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }
    }
}
