using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class StaticAvatarService : Abstract.BaseUserService<StaticAvatarService>
    {
        private readonly IStaticAvatarRepository _staticAvatarRepository;
        public StaticAvatarService(ILogger<StaticAvatarService> logger,
            IStaticAvatarRepository staticAvatarRepository,
            IUserContextProvider userContextProvider) : base(logger, userContextProvider)
        {
            _staticAvatarRepository = Require.IsNotNull(staticAvatarRepository,
                nameof(staticAvatarRepository));
        }

        public async Task<IEnumerable<StaticAvatar>> GetAvartarListAsync()
        {
            int siteId = GetClaimId(ClaimType.SiteId);
            return await _staticAvatarRepository.GetAvartarListAsync(siteId);
        }
    }
}
