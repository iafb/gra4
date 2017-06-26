using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class VendorCodeRepository
        : AuditingRepository<Model.VendorCode, VendorCode>, IVendorCodeRepository
    {
        public VendorCodeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<VendorCodeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        private static SemaphoreSlim AssignCodeSemaphore = new SemaphoreSlim(1, 1);

        public async Task<VendorCode> AssignCodeAsync(int vendorCodeTypeId, int userId)
        {
            await AssignCodeSemaphore.WaitAsync();
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .Where(_ => _.Id == userId)
                    .SingleOrDefaultAsync();

                var unusedCode = await DbSet
                    .Where(_ => _.SiteId == user.SiteId
                        && _.UserId == null)
                    .FirstOrDefaultAsync();

                if (unusedCode == null)
                {
                    _logger.LogCritical($"No available vendor codes of type {vendorCodeTypeId} to assign to {userId}.");
                    throw new Exception("No available vendor code to assign.");
                }

                unusedCode.UserId = userId;

                await _context.SaveChangesAsync();

                return await GetByIdAsync(unusedCode.Id);
            }
            finally
            {
                AssignCodeSemaphore.Release();
            }
        }

        public async Task<VendorCode> GetUserVendorCode(int userId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .OrderByDescending(_ => _.CreatedAt)
                .ProjectTo<VendorCode>()
                .FirstOrDefaultAsync();
        }

        public async Task<VendorCode> GetByCode(string code)
        {
            var vendorCode = await DbSet.AsNoTracking()
                .Where(_ => _.Code == code)
                .OrderByDescending(_ => _.CreatedAt)
                .FirstOrDefaultAsync();
            return _mapper.Map<VendorCode>(vendorCode);
        }
    }
}
