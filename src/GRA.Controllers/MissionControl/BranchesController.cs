using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Systems;
using GRA.Controllers.ViewModel.Shared;


namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageSystems)]
    public class BranchesController : Base.MCController
    {
        private readonly ILogger<BranchesController> _logger;
        private readonly SiteService _siteService;


        public BranchesController(ILogger<BranchesController> logger,
            ServiceFacade.Controller context,
            SiteService siteService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            PageTitle = "Branches";
        }

        
    }
}