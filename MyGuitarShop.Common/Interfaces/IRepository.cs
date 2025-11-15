using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Common.Interfaces
{
    public interface IRepository<TEntiy, TDto>
    {
        Task<IEnumerable<TEntiy>> GetAllAsync();
        Task<TEntiy?> FindByIdAsync(int id);
        Task<int> InsertAsync(TDto dto);
        Task<int> UpdateAsync(int id, TDto dto);
        Task<int> DeleteAsync(int id);
    }
}
