namespace MyGuitarShop.Common.Interfaces
{
    public interface IRepository<TEntiy, TDto>
    {
        Task<IEnumerable<TEntiy>> GetAllAsync();
        Task<TEntiy?> FindByIdAsync(string id);
        Task<bool> InsertAsync(TDto dto);
        Task<bool> UpdateAsync(string id, TDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
