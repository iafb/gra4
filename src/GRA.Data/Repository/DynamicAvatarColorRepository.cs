using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DynamicAvatarColorRepository : AuditingRepository<Model.DynamicAvatarColor, DynamicAvatarColor>,
        IDynamicAvatarColorRepository
    {
        public DynamicAvatarColorRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarColorRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<DynamicAvatarColor>> GetByLayerAsync(int layerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.DynamicAvatarLayerId == layerId)
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<DynamicAvatarColor>()
                .ToListAsync();
        }
    }
}
