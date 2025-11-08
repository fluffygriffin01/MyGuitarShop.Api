using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Common.Interfaces
{
    public interface IRepository<TDto>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> FindByIdAsync(int id);
        Task<int> InsertAsync(TDto dto);
        Task<int> UpdateAsync(int id, TDto dto);
        Task<int> DeleteAsync(int id);
    }
}
