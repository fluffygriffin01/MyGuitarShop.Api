using Microsoft.EntityFrameworkCore;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.EFCore.Context;

namespace MyGuitarShop.Data.EFCore.Abstract
{
    public abstract class RepositoryBase<TEntity>(
        MyGuitarShopContext dbContext)
        : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();


        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<TEntity?> FindByIdAsync(int id) =>
            await _dbSet.FindAsync(id);

        public async Task<int> InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(int id, TEntity entity)
        {
            var existingEntity = await FindByIdAsync(id);
            if (existingEntity == null)
                return 0;

            _dbSet.Entry(existingEntity).CurrentValues.SetValues(entity);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await FindByIdAsync(id);
            if (entity == null)
                return 0;

            _dbSet.Remove(entity);
            return await dbContext.SaveChangesAsync();
        }
    }
}
