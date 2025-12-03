using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Api.Mappers;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.Api.Abstract
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TDto, TEntity>(
        IRepository<TEntity> repository,
        ILogger<BaseController<TDto, TEntity>> logger
        ) : ControllerBase where TEntity : class, new()
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var entities = await repository.GetAllAsync();
                return entities.Any() ? Ok(entities) : NotFound("Entities not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetAsync");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var entity = await repository.FindByIdAsync(id);
                return entity != null ? Ok(entity) : NotFound($"Entity with {id} not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving entity with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TDto dto)
        {
            try
            {
                var entity = AutoReflectionMapper.Map<TDto, TEntity>(dto);
                if (entity == null)
                    throw new Exception("Mapping resulted in null entity");

                var numEntitiesCreated = await repository.InsertAsync(entity);
                return Ok($"{numEntitiesCreated} new entities created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating entity");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, TDto dto)
        {
            try
            {
                if (await repository.FindByIdAsync(id) == null)
                    return NotFound($"Entity with id {id} not found");

                var entity = AutoReflectionMapper.Map<TDto, TEntity>(dto);
                if (entity == null)
                    throw new Exception("Mapping resulted in null entity");

                var entitiesUpdated = await repository.UpdateAsync(id, entity);
                return Ok($"{entitiesUpdated} entities updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating entity");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (await repository.FindByIdAsync(id) == null)
                    return NotFound($"Entity with id {id} not found");

                var entitiesDeleted = await repository.DeleteAsync(id);
                return Ok($"{entitiesDeleted} entities deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting entity");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
