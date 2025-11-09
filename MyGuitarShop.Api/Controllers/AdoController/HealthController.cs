using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGuitarShop.Data.Ado.Factories;
using MyGuitarShop.Data.EFCore.Context;

namespace MyGuitarShop.Api.Controllers.AdoController
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController(
        ILogger<HealthController> logger,
        SqlConnectionFactory sqlConnectionFactory,
        MyGuitarShopContext dbContext) : ControllerBase
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
    }
}
