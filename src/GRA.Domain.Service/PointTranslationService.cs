using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class PointTranslationService : BaseUserService<PointTranslationService>
    {
        private readonly IPointTranslationRepository _pointTranslationRepository;
        public PointTranslationService(ILogger<PointTranslationService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPointTranslationRepository pointTranslationRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _pointTranslationRepository = pointTranslationRepository 
                ?? throw new ArgumentNullException(nameof(pointTranslationRepository));
        }

        public async Task<PointTranslation> GetByProgramIdAsync(int id, bool titleCase = false)
        {
            var pointTranslation = await _pointTranslationRepository.GetByProgramIdAsync(id);

            if (titleCase)
            {
                if (!string.IsNullOrWhiteSpace(pointTranslation.ActivityDescription))
                {
                    pointTranslation.ActivityDescription = pointTranslation.ActivityDescription
                        .First()
                        .ToString()
                        .ToUpper() 
                        + pointTranslation.ActivityDescription.Substring(1);
                }
                if (!string.IsNullOrWhiteSpace(pointTranslation.ActivityDescriptionPlural))
                {
                    pointTranslation.ActivityDescriptionPlural = 
                        pointTranslation.ActivityDescriptionPlural
                        .First()
                        .ToString()
                        .ToUpper()
                        + pointTranslation.ActivityDescriptionPlural.Substring(1);
                }
                if (!string.IsNullOrWhiteSpace(pointTranslation.TranslationDescriptionPastTense))
                {
                    pointTranslation.TranslationDescriptionPastTense =
                        pointTranslation.TranslationDescriptionPastTense
                        .First()
                        .ToString()
                        .ToUpper()
                        + pointTranslation.TranslationDescriptionPastTense.Substring(1);
                }
                if (!string.IsNullOrWhiteSpace(pointTranslation.TranslationDescriptionPresentTense))
                {
                    pointTranslation.TranslationDescriptionPresentTense =
                        pointTranslation.TranslationDescriptionPresentTense
                        .First()
                        .ToString()
                        .ToUpper()
                        + pointTranslation.TranslationDescriptionPresentTense.Substring(1);
                }
            }
            return pointTranslation;
        }
    }
}

﻿using System;
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
    public class PointTranslationService : BaseUserService<PointTranslationService>
    {
        private readonly IPointTranslationRepository _pointTranslationRepository;
        public PointTranslationService(ILogger<PointTranslationService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPointTranslationRepository pointTranslationRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _pointTranslationRepository = Require.IsNotNull(pointTranslationRepository,
                nameof(pointTranslationRepository));
        }

        public async Task<DataWithCount<ICollection<PointTranslation>>>
            GetPaginatedPointTranslationListAsync(BaseFilter filter)
        {
            VerifyPermission(Permission.ManagePointTranslations);
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<PointTranslation>>
            {
                Data = await _pointTranslationRepository.PageAsync(filter),
                Count = await _pointTranslationRepository.CountAsync(filter)
            };
        }

        public async Task<PointTranslation> AddPointTranslationAsync(PointTranslation pointTranslation)
        {
            VerifyPermission(Permission.ManagePointTranslations);
            pointTranslation.SiteId = GetCurrentSiteId();
            return await _pointTranslationRepository.AddSaveAsync(GetClaimId(ClaimType.UserId),
                pointTranslation);
        }

        public async Task UpdateProgramAsync(PointTranslation pointTranslation)
        {
            VerifyPermission(Permission.ManagePointTranslations);
            var currentPointTranslation = await _pointTranslationRepository.GetByIdAsync(
                pointTranslation.Id);
            if (currentPointTranslation.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - point translation belongs to site id {currentPointTranslation.SiteId}.");
            }

            currentPointTranslation.ActivityAmount = pointTranslation.ActivityAmount;
            currentPointTranslation.ActivityDescription = pointTranslation.ActivityDescriptionPlural;
            currentPointTranslation.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            currentPointTranslation.IsSingleEvent = pointTranslation.IsSingleEvent;
            currentPointTranslation.PointsEarned = pointTranslation.PointsEarned;
            currentPointTranslation.TranslationDescriptionPastTense = pointTranslation.TranslationDescriptionPastTense;
            currentPointTranslation.TranslationDescriptionPresentTense = pointTranslation.TranslationDescriptionPresentTense;
            currentPointTranslation.TranslationName = pointTranslation.TranslationName;

            await _pointTranslationRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId),
                currentPointTranslation);
        }

        public async Task RemovePointTranslationAsync(int pointTranslationId)
        {
            VerifyPermission(Permission.ManagePointTranslations);
            var pointTranslation = await _pointTranslationRepository.GetByIdAsync(pointTranslationId);
            if (pointTranslation.SiteId != GetCurrentSiteId())
            {
                throw new GraException($"Permission denied - point translation belongs to site id {pointTranslation.SiteId}.");
            }
            if (await _pointTranslationRepository.IsInUseAsync(pointTranslationId))
            {
                throw new GraException($"{pointTranslation.TranslationName} is in use by programs and/or challenge tasks.");
            }
            await _pointTranslationRepository.RemoveSaveAsync(GetActiveUserId(), pointTranslationId);
        }
    }
}
