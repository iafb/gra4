using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Sites;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageSites)]
    public class SitesController : Base.MCController
    {
        private readonly ILogger<SitesController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly SiteService _siteService;
        public SitesController(ILogger<SitesController> logger,
            ServiceFacade.Controller context,
            SiteService siteService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = context.Mapper ?? throw new ArgumentNullException(nameof(context.Mapper));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            PageTitle = "Site management";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var filter = new BaseFilter(page);

            var siteList = await _siteService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = siteList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new SiteListViewModel()
            {
                Sites = siteList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            var viewModel = _mapper.Map<Site, SiteDetailViewModel>(site);

            PageTitle = $"Site management - {site.Name}";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(SiteDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var site = await _siteLookupService.GetByIdAsync(model.Id);
                    _mapper.Map<SiteDetailViewModel, Site>(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(Detail), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            PageTitle = PageTitle = $"Site management - {model.Name}";
            return View(model);
        }

        public async Task<IActionResult> Configuration(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            var viewModel = _mapper.Map<Site, SiteConfigurationViewModel>(site);

            PageTitle = $"Site management - {site.Name}";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Configuration(SiteConfigurationViewModel model)
        {
            string siteName = null;
            if (ModelState.IsValid)
            {
                try
                {
                    var site = await _siteLookupService.GetByIdAsync(model.Id);
                    siteName = site.Name;
                    _mapper.Map<SiteConfigurationViewModel, Site>(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(Configuration), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            if (string.IsNullOrWhiteSpace(siteName))
            {
                var site = await _siteLookupService.GetByIdAsync(model.Id);
                siteName = site.Name;
            }
            PageTitle = PageTitle = $"Site management - {siteName}";
            return View(model);
        }

        public async Task<IActionResult> Schedule(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            var viewModel = _mapper.Map<Site, SiteScheduleViewModel>(site);

            PageTitle = $"Site management - {site.Name}";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Schedule(SiteScheduleViewModel model)
        {
            string siteName = null;
            if (ModelState.IsValid)
            {
                try
                {
                    var site = await _siteLookupService.GetByIdAsync(model.Id);
                    siteName = site.Name;
                    _mapper.Map<SiteScheduleViewModel, Site>(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(Schedule), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            if (string.IsNullOrWhiteSpace(siteName))
            {
                var site = await _siteLookupService.GetByIdAsync(model.Id);
                siteName = site.Name;
            }
            PageTitle = PageTitle = $"Site management - {siteName}";
            return View(model);
        }

        public async Task<IActionResult> SocialMedia(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            var viewModel = _mapper.Map<Site, SiteSocialMediaViewModel>(site);

            PageTitle = $"Site management - {site.Name}";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SocialMedia(SiteSocialMediaViewModel model)
        {
            string siteName = null;
            if (ModelState.IsValid)
            {
                try
                {
                    var site = await _siteLookupService.GetByIdAsync(model.Id);
                    siteName = site.Name;
                    _mapper.Map<SiteSocialMediaViewModel, Site>(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(SocialMedia), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            if (string.IsNullOrWhiteSpace(siteName))
            {
                var site = await _siteLookupService.GetByIdAsync(model.Id);
                siteName = site.Name;
            }
            PageTitle = PageTitle = $"Site management - {siteName}";
            return View(model);
        }

        public async Task<IActionResult> Settings(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            PageTitle = $"Site management - {site.Name}";
            return View(site);
        }
    }
}
