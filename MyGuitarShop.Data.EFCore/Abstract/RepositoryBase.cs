using Microsoft.EntityFrameworkCore;
using MyGuitarShop.Common.Mappers;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.EFCore.Context;

namespace MyGuitarShop.Data.EFCore.Abstract
{
    public abstract class RepositoryBase<TEntity, TDto>(
        MyGuitarShopContext dbContext)
        : IRepository<TEntity, TDto> where TEntity : class, new()
    {
        private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();


        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<TEntity?> FindByIdAsync(string id) =>
            await _dbSet.FindAsync(id);

        public async Task<bool> InsertAsync(TDto dto)
        {
            var entity = AutoReflectionMapper.Map<TDto, TEntity>(dto);
            if (entity == null)
                throw new Exception("Mapping resulted in null entity");

            await _dbSet.AddAsync(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(string id, TDto entity)
        {
            var existingEntity = await FindByIdAsync(id);
            if (existingEntity == null)
                return false;

            _dbSet.Entry(existingEntity).CurrentValues.SetValues(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await FindByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
