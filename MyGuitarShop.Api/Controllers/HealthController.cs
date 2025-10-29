using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController(
        ILogger<HealthController> logger,
        SqlConnectionFactory sqlConnectionFactory) : ControllerBase
    {
        [HttpGet]
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
        }
    }
}
