using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyGuitarShop.Data.Ado.Factories;
using MyGuitarShop.Data.EFCore.Context;

namespace MyGuitarShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController(
        ILogger<HealthController> logger,
        SqlConnectionFactory sqlConnectionFactory,
        MyGuitarShopContext dbContext,
        IMongoClient mongoClient) : ControllerBase
    {
        /*[HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok("Healthy");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Health check failed.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service Unavailable");
            }
        }

        [HttpGet(template: "db")]
        public IActionResult GetDbHealth()
        {
            try
            {
                using var connection = sqlConnectionFactory.OpenSqlConnection();
                return Ok(new { Message = "Connection Successful", connection.Database });
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Database health check failed.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Database Unavailable");
            }
        }*/

        [HttpGet("db/ado")]
        public IActionResult GetDbHealth()
        {
            try
            {
                using var connection = sqlConnectionFactory.OpenSqlConnection();
                return Ok(new { Message = "Connection Successful", connection.Database });
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Database health check failed.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Database Unhealthy");
            }
        }

        [HttpGet("db/efcore")]
        public async Task<IActionResult> GetDBContextHealthAsync()
        {
            try
            {
                var canConnect = await dbContext.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok(new { Message = "EF Core Connection Successful", dbContext.Database });
                }
                else
                {
                    logger.LogCritical("EF Core cannot connect to the database.");
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, "EF Core Database Unavailable");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "EF Core database health check failed.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "EF Core Database Unavailable");
            }
        }

        [HttpGet("db/mongo")]
        public async Task<IActionResult> GetMongoDBHealthAsync()
        {
            try
            {
                var response = await mongoClient.ListDatabaseNamesAsync();
                var databaseNames = await response.ToListAsync() ?? [];

                if (databaseNames.Count <= 0)
                    throw new Exception("Cannot connect to Mongo database.");

                return Ok(new { Message = "MongoDB Connection Successful", Databases = databaseNames });
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "MongoDB database health check failed.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "MongoDB Database Unavailable");
            }
        }
    }
}
