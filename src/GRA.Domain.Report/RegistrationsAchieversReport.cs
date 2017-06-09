using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Report.Abstract;
using GRA.Domain.Report.Attribute;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Report
{
    [ReportInformation(2,
    "Registrations and Achievers Report",
    "Registered participants and achievers by branch (filterable by system and date).",
    "Program")]
    public class RegistrationsAchieversReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserRepository _userRepository;
        public RegistrationsAchieversReport(ILogger<RegistrationsAchieversReport> logger,
            ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            ISystemRepository systemRepository,
            IUserRepository userRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentNullException(nameof(systemRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<OperationStatus> progress = null)
        {
            #region Reporting initialization
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request = await StartRequestAsync(request);

            var criterion
                = await _serviceFacade.ReportCriterionRepository.GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            var report = new StoredReport
            {
                Title = ReportAttribute?.Name,
                AsOf = _serviceFacade.DateTimeProvider.Now
            };
            var reportData = new List<object[]>();
            #endregion Reporting initialization

            #region Adjust report criteria as needed
            // collect systems - all if none is specified
            ICollection<int> systemIds = null;
            if (criterion.SystemId == null)
            {
                var systems = await _systemRepository.GetAllAsync((int)criterion.SiteId);
                systemIds = systems.Select(_ => _.Id).ToList();
            }
            else
            {
                systemIds = new List<int>();
                systemIds.Add((int)criterion.SystemId);
            }
            #endregion Adjust report criteria as needed

            #region Collect data
            UpdateProgress(progress, 1, "Starting report...");

            // header row
            var row = new List<object>();
            row.Add("System Name");
            row.Add("Branch Name");
            row.Add("Registered Users");
            row.Add("Achievers");
            report.HeaderRow = row.ToArray();

            int count = 0;

            // running totals
            long totalRegistered = 0;
            long totalAchiever = 0;

            foreach (var systemId in systemIds)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                UpdateProgress(progress, (++count * 100) / systemIds.Count);

                var branches = await _branchRepository.GetBySystemAsync(systemId);
                foreach (var branch in branches)
                {
                    UpdateProgress(progress, $"Processing: {branch.SystemName} - {branch.Name}");

                    criterion.SystemId = systemId;
                    criterion.BranchId = branch.Id;

                    int users = await _userRepository.GetCountAsync(criterion);
                    int achievers = await _userRepository.GetAchieverCountAsync(criterion);
                    totalRegistered += users;
                    totalAchiever += achievers;

                    // add row
                    row = new List<object>();
                    row.Add(branch.SystemName);
                    row.Add(branch.Name);
                    row.Add(users);
                    row.Add(achievers);

                    reportData.Add(row.ToArray());

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }

            report.Data = reportData.ToArray();

            // total row
            row = new List<object>();
            row.Add("Total");
            row.Add(string.Empty);
            row.Add(totalRegistered);
            row.Add(totalAchiever);
            report.FooterRow = row.ToArray();
            #endregion Collect data

            #region Finish up reporting
            _logger.LogInformation($"Report {GetType().Name} with criterion {criterion.Id} ran in {StopTimer()}");

            request.Success = !token.IsCancellationRequested;

            if (request.Success == true)
            {
                ReportSet.Reports.Add(report);
                request.Finished = _serviceFacade.DateTimeProvider.Now;
                request.ResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(ReportSet);
            }
            await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
            #endregion Finish up reporting
        }
    }
}
