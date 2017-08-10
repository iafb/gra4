using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class ProgramRepository
        : AuditingRepository<Model.Program, Program>, IProgramRepository
    {
        public ProgramRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<ProgramRepository> logger) : base(repositoryFacade, logger)
        {
        }
        public async Task<IEnumerable<Program>> GetAllAsync(int siteId)
        {
            return await DbSet
               .AsNoTracking()
               .Where(_ => _.SiteId == siteId)
               .OrderBy(_ => _.Position)
               .ProjectTo<Program>()
               .ToListAsync();
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<Program>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<Program>()
                .ToListAsync();
        }

        private IQueryable<Model.Program> ApplyFilters(BaseFilter filter)
        {
            var programList = DbSet
                 .AsNoTracking()
                 .Where(_ => _.SiteId == filter.SiteId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                programList = programList.Where(_ => _.Name.Contains(filter.Search));
            }

            return programList;
        }

        public async Task<bool> IsInUseAsync(int programId)
        {
            return await _context.Users.AsNoTracking()
                .AnyAsync(_ => _.ProgramId == programId && _.IsDeleted);
        }

        public async Task<bool> ValidateAsync(int programId, int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == programId && _.SiteId == siteId)
                .AnyAsync();
        }
    }
}
