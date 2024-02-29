using Dapper;
using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{

    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductController(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }
       /// <summary>
       /// Get all products
       /// </summary>
       /// <returns></returns>
        [HttpGet("/products",Name =nameof(GetAllProducts))]
        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                var result = _sqlExecutor.Execute<IEnumerable<Product>>(
                    (connection, transaction) =>
                    {
                        return connection.Query<Product>(
                            sql: @"SELECT * FROM Instances.Products",
                            transaction: transaction
                        );
                    }
                );

                return (IEnumerable<Product>)Ok(result);
            }
            catch (Exception ex)
            {
                return (IEnumerable<Product>)StatusCode(500, $"An error occurred while retrieving all products: {ex.Message}");
            }
        }
        /// <summary>
        /// Create new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost("/products/create",Name =nameof(CreateProduct))]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        return connection.Execute(
                            @"INSERT INTO Instances.Products (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) 
                            VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp)",
                            product,
                            transaction
                        );
                    }
                );

                if (numberOfRowsAffected > 0)
                    return Ok();
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }

        /// <summary>
        /// Get product detail by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("/products/instanceId/{name}")]
        public IActionResult GetProductInstanceIdFromName(string name)
        {
            try
            {
                var result = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        return connection.QueryFirstOrDefault<int>(
                            sql: @"SELECT top 1 InstanceId FROM [master].[Instances].[Products] 
                                WHERE name=@name",
                            new { Name = name },
                            transaction: transaction
                        );
                    }
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving product details: {ex.Message}");
            }
        }


    }
}
