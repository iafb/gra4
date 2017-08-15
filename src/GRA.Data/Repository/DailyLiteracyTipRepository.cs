using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DailyLiteracyTipRepository 
        : AuditingRepository<Model.DailyLiteracyTip, DailyLiteracyTip>, IDailyLiteracyTipRepository
    {
        public DailyLiteracyTipRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DailyLiteracyTipRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<DailyLiteracyTip>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .ApplyPagination(filter)
                .ProjectTo<DailyLiteracyTip>()
                .ToListAsync();
        }

        private IQueryable<Model.DailyLiteracyTip> ApplyFilters(BaseFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);
        }
    }
}
