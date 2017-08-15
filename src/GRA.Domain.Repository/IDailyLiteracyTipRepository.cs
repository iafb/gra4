using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDailyLiteracyTipRepository : IRepository<DailyLiteracyTip>
    {
        Task<int> CountAsync(BaseFilter filter);
        Task<ICollection<DailyLiteracyTip>> PageAsync(BaseFilter filter);
    }
}
