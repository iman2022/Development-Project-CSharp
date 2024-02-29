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
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {
        private readonly ISqlExecutor _sqlExecutor;

        public InventoryController(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }
        /// <summary>
        /// Creaye new inventory
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        [HttpPost("/inventory/create",Name =nameof(CreateInventory))]
        public IActionResult CreateInventory([FromBody] Inventory inventory)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        string query = @"INSERT INTO [master].[Transactions].[InventoryTransactions] 
                             ([ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory])
                             VALUES (@ProductInstanceId, @Quantity, @StartedTimestamp, @CompletedTimestamp, @TypeCategory)";

                        return connection.Execute(query, inventory, transaction);
                    }
                );

                if (numberOfRowsAffected > 0)
                {
                    // If rows were affected, return Ok
                    return Ok();
                }
                else
                {
                    // If no rows were affected, return an appropriate response (e.g., NotFound or BadRequest)
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }
        /// <summary>
        ///  Creaye multiple inventories
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        [HttpPost("/inventory/createmore", Name = nameof(CreateMultipleInventories))]
        public IActionResult CreateMultipleInventories([FromBody] Inventory[] inventory)
        {
            try
            {
                int numberOfRowsAffected = 0;
                foreach (Inventory inventoryItem in inventory)
                {
                    int rowsAffected = _sqlExecutor.Execute<int>(
                        (connection, transaction) =>
                        {
                            string query = @"INSERT INTO [master].[Transactions].[InventoryTransactions] 
                             ([ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory])
                             VALUES (@ProductInstanceId, @Quantity, @StartedTimestamp, @CompletedTimestamp, @TypeCategory)";

                            return connection.Execute(query, inventoryItem, transaction);
                        }
                    );
                    numberOfRowsAffected++;
                }

                if (numberOfRowsAffected > 0)
                {
                    // If rows were affected, return Ok
                    return Ok(numberOfRowsAffected);
                }
                else
                {
                    // If no rows were affected, return an appropriate response (e.g., NotFound or BadRequest)
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"An error occurred while adding  products to inventory: {ex.Message}");
            }
        }
        /// <summary>
        /// Get inventory count
        /// </summary>
        /// <param name="productInstanceId"></param>
        /// <returns></returns>
        [HttpGet("/inventory/count", Name = nameof(GetInventoryCount))]
        public IActionResult GetInventoryCount(string productInstanceId)
        {
            try
            {
                var quantityResult = _sqlExecutor.Execute<decimal>(
                    (connection, transaction) =>
                    {
                        string query = @"SELECT top 1 [Quantity]
                             FROM [master].[Transactions].[InventoryTransactions]
                             WHERE ProductInstanceId=@ProductInstanceId";

                        return connection.QueryFirstOrDefault<decimal>(query, new { ProductInstanceId = productInstanceId }, transaction);
                    }
                );

                return Ok(quantityResult);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"An error occurred while retrieving inventory count: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove inventory by id
        /// </summary>
        /// <param name="productInstanceId"></param>
        /// <returns></returns>
        [HttpDelete("/inventory/delete",Name = nameof(RemoveInventoryById))]
        public IActionResult RemoveInventoryById(string productInstanceId)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        string query = @"DELETE [master].[Transactions].[InventoryTransactions]
                             WHERE ProductInstanceId=@ProductInstanceId";

                        return connection.Execute(query, new { ProductInstanceId = productInstanceId }, transaction);
                    }
                );

                if (numberOfRowsAffected > 0)
                {
                    // If rows were affected, return Ok
                    return Ok();
                }
                else
                {
                    // If no rows were affected, return an appropriate response (e.g., NotFound or BadRequest)
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"An error occurred while deleting inventory: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove more inventories
        /// </summary>
        /// <param name="productInstanceIds"></param>
        /// <returns></returns>
        [HttpPost("/inventory/deletemore", Name = nameof(RemoveInventoryByIds))]
        public IActionResult RemoveInventoryByIds(string[] productInstanceIds)
        {
            try
            {
                int numberOfRowsAffected = 0;
                for (int i = 0; i < productInstanceIds.Length; i++)
                {
                    int rowsAffected = _sqlExecutor.Execute<int>(
                        (connection, transaction) =>
                        {
                            string query = @"DELETE [master].[Transactions].[InventoryTransactions]
                             WHERE ProductInstanceId=@ProductInstanceId";

                            return connection.Execute(query, new { ProductInstanceId = productInstanceIds[i] }, transaction);
                        }
                    );
                    numberOfRowsAffected++;
                }

                if (numberOfRowsAffected > 0)
                {
                    // If rows were affected, return Ok
                    return Ok();
                }
                else
                {
                    // If no rows were affected, return an appropriate response (e.g., NotFound or BadRequest)
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"An error occurred while deleting inventory: {ex.Message}");
            }
        }

    }
}
