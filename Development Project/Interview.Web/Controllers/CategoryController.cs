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
    [Route("api/v1/categories")]
    public class CategoryController : Controller
    {
        private readonly ISqlExecutor _sqlExecutor;

        public CategoryController(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }
        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost("/categories/create",Name =nameof(CreateCategory))]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        return connection.Execute(
                            @"INSERT INTO Instances.Categories (Name, Description, CreatedTimestamp) 
                            VALUES (@Name, @Description, @CreatedTimestamp)",
                            category,
                            transaction
                        );
                    }
                );

                if (numberOfRowsAffected > 0)
                {
                    // Fetch the InstanceId of the newly inserted category
                    int instanceId = _sqlExecutor.Execute<int>(
                        (connection, transaction) =>
                        {
                            return connection.QueryFirstOrDefault<int>(
                                "SELECT SCOPE_IDENTITY()",
                                transaction: transaction
                            );
                        }
                    );

                    return Ok(instanceId);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating new category: {ex.Message}");
            }
        }
    }
}
