using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class DailyLiteracyTipService : BaseUserService<DailyLiteracyTipService>
    {
        private readonly IDailyLiteracyTipRepository _dailyLiteracyTipRepository;
        public DailyLiteracyTipService(ILogger<DailyLiteracyTipService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IDailyLiteracyTipRepository dailyLiteracyTipRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageDailyLiteracyTips);
            _dailyLiteracyTipRepository = Require.IsNotNull(dailyLiteracyTipRepository,
                nameof(dailyLiteracyTipRepository));
        }

        public async Task<DataWithCount<ICollection<DailyLiteracyTip>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<DailyLiteracyTip>>
            {
                Data = await _dailyLiteracyTipRepository.PageAsync(filter),
                Count = await _dailyLiteracyTipRepository.CountAsync(filter)
            };
        }

        public async Task<DailyLiteracyTip> AddAsync(DailyLiteracyTip dailyLiteracyTip)
        {
            VerifyManagementPermission();

            dailyLiteracyTip.SiteId = GetCurrentSiteId();

            return await _dailyLiteracyTipRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                dailyLiteracyTip);
        }

        public async Task UpdateAsync(DailyLiteracyTip dailyLiteracyTip)
        {
            VerifyManagementPermission();

            var currentDailyLiteracyTip = await _dailyLiteracyTipRepository.GetByIdAsync(
                dailyLiteracyTip.Id);
            if (currentDailyLiteracyTip.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - Daily Literacy Tip belongs to site id {currentDailyLiteracyTip.SiteId}");
            }

            currentDailyLiteracyTip.Message = dailyLiteracyTip.Message;
            currentDailyLiteracyTip.Name = dailyLiteracyTip.Name;

            await _dailyLiteracyTipRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentDailyLiteracyTip);
        }

        public async Task<DailyLiteracyTipImage> GetImageByDayAsync(int day)
        {
            return null;
        }
    }
}
