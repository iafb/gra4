using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<int> CountAsync(BaseFilter filter);
        Task<IEnumerable<Category>> PageAsync(BaseFilter filter);
    }
}