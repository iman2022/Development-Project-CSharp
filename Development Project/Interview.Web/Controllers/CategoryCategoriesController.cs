using Dapper;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.SqlServer.Abstractions;
using System;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/categorycategories")]
    public class CategoryCategoriesController : Controller
    {
        private readonly ISqlExecutor _sqlExecutor;

        public CategoryCategoriesController(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }
        /// <summary>
        /// Add new category categories
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="categoryInstanceId"></param>
        /// <returns></returns>
        [HttpPost("/categorycategories/add", Name = nameof(AddCategoryCategories))]
        public IActionResult AddCategoryCategories(int instanceId, int categoryInstanceId)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        return connection.Execute(
                            @"INSERT INTO Instances.CategoryCategories(InstanceId, CategoryInstanceId) 
                            VALUES (@InstanceId, @CategoryInstanceId)",
                            new { InstanceId = instanceId, CategoryInstanceId = categoryInstanceId },
                            transaction
                        );
                    }
                );

                if (numberOfRowsAffected > 0)
                {
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
                return StatusCode(500, $"An error occurred while adding the category categories: {ex.Message}");
            }
        }
    }
}
