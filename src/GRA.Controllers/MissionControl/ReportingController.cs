using GRA.Controllers.ViewModel.MissionControl.Reporting;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewAllReporting)]
    public class ReportingController : Base.MCController
    {
        private readonly ILogger<ReportingController> _logger;
        private readonly ReportService _reportService;
        private readonly SiteService _siteService;

        public ReportingController(ILogger<ReportingController> logger,
            ServiceFacade.Controller context,
            ReportService reportService,
            SiteService siteService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _reportService = Require.IsNotNull(reportService, nameof(reportService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            PageTitle = "Reporting";
        }

        [HttpGet]
        public IActionResult Index()
        {
            PageTitle = "Select a Report";
            return View(_reportService.GetReportList());
        }

        [HttpGet]
        public async Task<IActionResult> Configure(int id)
        {
            PageTitle = "Configure the Report";
            var report = _reportService.GetReportList().Where(_ => _.Id == id).SingleOrDefault();
            if (report == null)
            {
                AlertDanger = $"Could not find report of type {id}.";
                return RedirectToAction("Index");
            }
            string viewName = report.Name.Replace(" ", string.Empty);
            if (viewName.EndsWith("Report"))
            {
                viewName = viewName.Substring(0, viewName.Length - 6);
            }

            var systemList = await _siteService.GetSystemList();

            return View($"{viewName}Criteria", new ReportCriteriaViewModel
            {
                ReportId = id,
                SystemList = new SelectList(systemList, "Id", "Name"),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Run(ReportCriteriaViewModel viewModel)
        {
            PageTitle = "Run the report";

            var criterion = new ReportCriterion
            {
                SiteId = GetCurrentSiteId(),
                EndDate = viewModel.EndDate,
                StartDate = viewModel.StartDate,
                SystemId = viewModel.SystemId,
                BranchId = viewModel.BranchId,
                ProgramId = viewModel.ProgramId,
            };

            var reportRequestId = await _reportService
                .RequestReport(criterion, viewModel.ReportId);

            var wsUrl = await _siteService.GetWsUrl(Request.Scheme, Request.Host.Value);

            return View("Run", new RunReportViewModel
            {
                Id = reportRequestId,
                RunReportUrl = $"{wsUrl}/MissionControl/runreport/{reportRequestId}"
            });
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            try
            {
                var storedReport = await _reportService.GetReportResultsAsync(id);

                PageTitle = storedReport.request.Name ?? "Report Results";

                var viewModel = new ReportResultsViewModel
                {
                    Title = storedReport.request.Name ?? "Report Results",
                };

                viewModel.ReportSet = JsonConvert
                    .DeserializeObject<StoredReportSet>(storedReport.request.ResultJson);

                foreach(var report in viewModel.ReportSet.Reports)
                {
                    int count = 0;
                    int totalRows = report.Data.Count();
                    var displayRows = new List<List<string>>();

                    if(report.HeaderRow != null)
                    {
                        var display = new List<string>();
                        foreach(var dataItem in report.HeaderRow)
                        {
                            display.Add(FormatDataItem(dataItem));
                        }
                        report.HeaderRow = display;
                    }

                    foreach (var resultRow in report.Data)
                    {
                        var displayRow = new List<string>();

                        foreach (var resultItem in resultRow)
                        {
                            displayRow.Add(FormatDataItem(resultItem));
                        }
                        displayRows.Add(displayRow);
                        count++;
                    }
                    report.Data = displayRows;

                    if (report.FooterRow != null)
                    {
                        var display = new List<string>();
                        foreach (var dataItem in report.FooterRow)
                        {
                            display.Add(FormatDataItem(dataItem));
                        }
                        report.FooterRow = display;
                    }
                }

                return View(viewModel);
            }
            catch (GraException gex)
            {
                AlertDanger = gex.Message;
                return RedirectToAction("Index");
            }
        }

        private string FormatDataItem(object dataItem)
        {
            switch (dataItem)
            {
                case int i:
                    return i.ToString("N0");
                case long l:
                    return l.ToString("N0");
                default:
                    return dataItem.ToString();
                case null:
                    return string.Empty;
            }
        }
    }
}
