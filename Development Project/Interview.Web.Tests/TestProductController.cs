using Interview.Web.Controllers;
using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Configuration;
using Xunit;

namespace Interview.Web.Tests
{
    public class TestProductController : Controller
    {
        private readonly ISqlExecutor _sqlExecutor;
       
        public TestProductController()
        {
            _sqlExecutor = new SqlServerExecutor("Server=localhost\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;");
        }
        [Fact]
        public void GetAllProducts_ShouldReturnAllProducts()
        {
            var testProducts = 2;
            var controller = new ProductController(_sqlExecutor);

            List<Product> result = (List<Product>)controller.GetAllProducts();
            Assert.Equal(testProducts, result.Count);
        }
    }
}