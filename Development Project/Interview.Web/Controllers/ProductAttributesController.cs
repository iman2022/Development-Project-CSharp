using Dapper;
using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Interview.Web.Controllers
{

    [ApiController]
    [Route("api/v1/productattributes")]
    public class ProductAttributesController : Controller
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductAttributesController(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }
        /// <summary>
        /// Add new product attributes
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        [HttpPost("/productattributes/add",Name =nameof(AddProductAttributes))]
        public IActionResult AddProductAttributes(int instanceId, string key, string value)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        return connection.Execute(
                            @"INSERT INTO Instances.ProductAttributes (InstanceId, Key, Value) 
                            VALUES (@InstanceId, @Key, @Value)",
                            new { InstanceId = instanceId, Key = key, Value = value },
                            transaction
                        );
                    }
                );

                if (numberOfRowsAffected > 0)
                {
                    // Fetch the InstanceId of the newly inserted category
                    int id = _sqlExecutor.Execute<int>(
                        (connection, transaction) =>
                        {
                            return connection.QueryFirstOrDefault<int>(
                                "SELECT SCOPE_IDENTITY()",
                                transaction: transaction
                            );
                        }
                    );

                    return Ok(id);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding product attribute: {ex.Message}");
            }
        }
    }
}
