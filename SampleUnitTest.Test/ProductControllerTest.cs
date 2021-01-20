﻿using Microsoft.AspNetCore.Mvc;
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

        [Fact]
        public async void Detail_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _productController.Detail(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Detail_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _mockRepository.Setup(x => x.GetById(0)).ReturnsAsync(product);

            var result = await _productController.Detail(0);
            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Detail_ValidId_ReturnProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);
            _mockRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productController.Detail(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _productController.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_ReturnView()
        {
            _productController.ModelState.AddModelError("Name", "Name alanı gereklidir.");

            var result = await _productController.Create(products.First());
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _productController.Create(products.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {
            Product product = null;
            _mockRepository.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => product = x);

            var result = await _productController.Create(products.First());

            _mockRepository.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
            Assert.Equal(products.First().Id, product.Id);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_NeverCreateExecute()
        {
            _productController.ModelState.AddModelError("Name", "");

            var result = await _productController.Create(products.First());
            _mockRepository.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);
        }

    }
}
