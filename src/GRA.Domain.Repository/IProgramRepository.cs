using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IProgramRepository : IRepository<Program>
    {
        Task<IEnumerable<Program>> GetAllAsync(int siteId);
        Task<ICollection<Program>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
        Task<bool> IsInUseAsync(int programId);
        Task<bool> ValidateAsync(int programId, int siteId);
    }
}
