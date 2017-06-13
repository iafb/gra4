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
    public class BadgeRepository
        : AuditingRepository<Model.Badge, Domain.Model.Badge>, IBadgeRepository
    {
        public BadgeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<BadgeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task AddUserBadge(int userId, int badgeId)
        {
            if (!await UserHasBadge(userId, badgeId))
            {
                _context.UserBadges.Add(new Model.UserBadge
                {
                    UserId = userId,
                    BadgeId = badgeId,
                    CreatedAt = _dateTimeProvider.Now
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountForUserAsync(int userId)
        {
            return await _context.UserBadges
                .AsNoTracking()
                .Include(_ => _.Badge)
                .Where(_ => _.UserId == userId)
                .CountAsync();
        }

        public async Task<IEnumerable<Badge>> PageForUserAsync(int userId, int skip, int take)
        {
            return await _context.UserBadges
                .AsNoTracking()
                .Include(_ => _.Badge)
                .Where(_ => _.UserId == userId)
                .OrderByDescending(_ => _.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(_ => _.Badge)
                .ProjectTo<Badge>()
                .ToListAsync();
        }

        public async Task<bool> UserHasBadge(int userId, int badgeId)
        {
            return null != await _context.UserBadges
                .Where(_ => _.UserId == userId && _.BadgeId == badgeId)
                .SingleOrDefaultAsync();
        }

        public async Task RemoveUserBadgeAsync(int userId, int badgeId)
        {
            var userBadge = await _context.UserBadges
                .Where(_ => _.UserId == userId && _.BadgeId == badgeId)
                .SingleOrDefaultAsync();

            if (userBadge != null)
            {
                _context.UserBadges.Remove(userBadge);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GetBadgeNameAsync(int badgeId)
        {
            var trigger = await _context.Triggers
                .AsNoTracking()
                .Where(_ => _.AwardBadgeId == badgeId)
                .FirstOrDefaultAsync();

            if (trigger != null)
            {
                return trigger.Name;
            }

            var program = await _context.Programs
                .AsNoTracking()
                .Where(_ => _.JoinBadgeId == badgeId || _.AchieverBadgeId == badgeId)
                .FirstOrDefaultAsync();

            if (program != null)
            {
                return program.JoinBadgeId == badgeId
                    ? $"Joined {program.Name}"
                    : $"Achiever status in {program.Name}";
            }

            var questionnaire = await _context.Questionnaires
                .AsNoTracking()
                .Where(_ => _.BadgeId == badgeId)
                .FirstOrDefaultAsync();

            if (questionnaire != null)
            {
                return $"Completed questionnaire {questionnaire.Name}";
            }

            return $"Badge id {badgeId}";
        }
    }
}
