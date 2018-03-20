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
    public class AvatarItemRepository : AuditingRepository<Model.AvatarItem, AvatarItem>,
        IAvatarItemRepository
    {
        public AvatarItemRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AvatarItemRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<AvatarItem>> GetByLayerAsync(int layerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayerId == layerId)
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<AvatarItem>()
                .ToListAsync();
        }

        public async Task<ICollection<AvatarItem>> GetUserItemsByLayerAsync(int userId,
            int layerId)
        {
            var userUnlockedItems = _context.UserAvatarItems.AsNoTracking()
                .Where(_ => _.UserId == userId 
                    && _.AvatarItem.AvatarLayerId == layerId)
                .Select(_ => _.AvatarItem);

            return await DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayerId == layerId 
                && (_.Unlockable == false || userUnlockedItems.Select(u => u.Id).Contains(_.Id)))
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<AvatarItem>()
                .ToListAsync();
        }

        public async Task<bool> HasUserUnlockedItemAsync(int userId, int itemId)
        {
            return await _context.UserAvatarItems.AsNoTracking()
                .Where(_ => _.UserId == userId && _.AvatarItemId == itemId)
                .AnyAsync();
        }

        public async Task<ICollection<int>> GetUserUnlockedItemsAsync(int userId)
        {
            return await _context.UserAvatarItems.AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.AvatarItemId)
                .ToListAsync();
        }

        public async Task AddUserItemsAsync(int userId, List<int> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                await _context.UserAvatarItems.AddAsync(new Model.UserAvatarItem()
                {
                    UserId = userId,
                    AvatarItemId = itemId
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<AvatarItem>> PageAsync(AvatarFilter filter)
        {
            return await ApplyFilters(filter)
                .ApplyPagination(filter)
                .ProjectTo<AvatarItem>()
                .ToListAsync();
        }

        private IQueryable<Model.AvatarItem> ApplyFilters(AvatarFilter filter)
        {
            var items = DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayer.SiteId == filter.SiteId);

            if (filter.Unlockable.HasValue)
            {
                items = items.Where(_ => _.Unlockable == filter.Unlockable.Value);
            }

            if (filter.LayerId.HasValue)
            {
                items = items.Where(_ => _.AvatarLayerId == filter.LayerId.Value);
            }

            if (filter.ItemIds?.Count > 0)
            {
                items = items.Where(_ => !filter.ItemIds.Contains(_.Id));
            }

            if (!string.IsNullOrEmpty(filter.Search))
            {
                items = items.Where(_ => _.Name.Contains(filter.Search));
            }

            return items;
        }

        public async Task<ICollection<AvatarItem>> GetByIdsAsync(List<int> ids)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => ids.Contains(_.Id))
                .ProjectTo<AvatarItem>()
                .ToListAsync();
        }
    }
}
