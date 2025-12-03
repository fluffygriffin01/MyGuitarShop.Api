using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Common.Mappers;
using SharpCompress.Common;

namespace MyGuitarShop.Data.MongoDb.Abstract
{
    public abstract class MongoService<TEntity, TDto>(
        IMongoDatabase database) 
        : IRepository<TEntity, TDto> where TEntity : MongoModel, new()
    {
        private IMongoCollection<TEntity> Entities => database.GetCollection<TEntity>(nameof(TEntity));


        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Entities.Find(_ => true).ToListAsync();
        }

        public async Task<TEntity?> FindByIdAsync(string id)
        {
            return await Entities.Find(f => Equals(f._id, id)).FirstOrDefaultAsync();
        }

        public async Task<bool> InsertAsync(TDto dto)
        {
            var entity = AutoReflectionMapper.Map<TDto, TEntity>(dto);
            if (entity == null)
                throw new Exception("Mapping resulted in null entity");

            await Entities.InsertOneAsync(entity);
            return !string.IsNullOrEmpty(entity._id);
        }

        public async Task<bool> UpdateAsync(string id, TDto dto)
        {
            var entity = AutoReflectionMapper.Map<TDto, TEntity>(dto);
            if (entity == null)
                throw new Exception("Mapping resulted in null entity");

            var result = await Entities.ReplaceOneAsync(f => Equals(f._id, id), entity);
            return result.IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await Entities.DeleteOneAsync(f => Equals(f._id, id));
            return result.IsAcknowledged;
        }
    }
}
