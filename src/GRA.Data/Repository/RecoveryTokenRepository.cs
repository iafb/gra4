using GRA.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Data.ServiceFacade;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class RecoveryTokenRepository
        : AuditingRepository<Model.RecoveryToken, Domain.Model.RecoveryToken>,
        IRecoveryTokenRepository
    {
        public RecoveryTokenRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<RecoveryTokenRepository> logger) : base(repositoryFacade, logger)
        {
        }

        private async Task<bool> UserHasTokens(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .CountAsync() > 0;
        }

        public async Task<IEnumerable<RecoveryToken>> GetByUserIdAsync(int userId)
        {
            if (await UserHasTokens(userId))
            {
                return await DbSet
                    .AsNoTracking()
                    .Where(_ => _.UserId == userId)
                    .OrderByDescending(_ => _.CreatedAt)
                    .ProjectTo<RecoveryToken>()
                    .ToListAsync();
            }
            else
            {
                return new List<RecoveryToken>();
            }
        }
    }
}
