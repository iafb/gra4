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
    [ReportInformation(3,
    "Participant Progress Report",
    "See participant progress by system and date (total registrations, earned 250, 500, 750, 1000 over the provided time period).",
    "Program")]
    public class ParticipantProgressReport : BaseReport
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;

        public ParticipantProgressReport(ILogger<ParticipantProgressReport> logger,
            ServiceFacade.Report serviceFacade,
            IBranchRepository branchRepository,
            IProgramRepository programRepository,
            ISystemRepository systemRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger, serviceFacade)
        {
            _branchRepository = branchRepository
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _programRepository = programRepository
                ?? throw new ArgumentNullException(nameof(programRepository));
            _systemRepository = systemRepository
                ?? throw new ArgumentNullException(nameof(systemRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _userLogRepository = userLogRepository
                ?? throw new ArgumentNullException(nameof(userLogRepository));
        }

        public override async Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<OperationStatus> progress = null)
        {
            var pointValues = new long[] { 250, 500, 750, 1000 };

            #region Reporting initialization
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request = await StartRequestAsync(request);

            var criterion
                = await _serviceFacade.ReportCriterionRepository.GetByIdAsync(request.ReportCriteriaId)
                ?? throw new GraException($"Report criteria {request.ReportCriteriaId} for report request id {request.Id} could not be found.");

            var report = new StoredReport();
            var reportData = new List<object[]>();
            #endregion Reporting initialization

            #region Adjust report criteria as needed
            if (criterion.StartDate == null)
            {
                criterion.StartDate = DateTime.MinValue;
            }
            if (criterion.EndDate == null)
            {
                criterion.EndDate = DateTime.MaxValue;
            }

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
            UpdateProgress(progress, 1, "Getting total user count...");

            var totalCheck = new Dictionary<long, long>();

            // header row
            var row = new List<object>();
            row.Add("System Name");
            row.Add("Branch Name");
            row.Add("Program");
            row.Add("Registered Users");
            foreach (var pointValue in pointValues)
            {
                row.Add($"Achieved {pointValue} points");
                totalCheck.Add(pointValue, 0);
            }
            report.HeaderRow = row.ToArray();

            long totalRegistered = 0;
            long count = 0;

            var programIds = await _programRepository.GetAllAsync((int)criterion.SiteId);

            long reportUserCount = 0;
            foreach (var systemId in systemIds)
            {
                var userCriterion = new ReportCriterion
                {
                    EndDate = criterion.EndDate,
                    ProgramId = criterion.ProgramId,
                    SiteId = criterion.SiteId,
                    StartDate = criterion.StartDate,
                    SystemId = systemId
                };
                reportUserCount += await _userRepository.GetCountAsync(userCriterion);
            }

            UpdateProgress(progress, $"Beginning processing of {reportUserCount} users...");

            foreach (var systemId in systemIds)
            {
                var branches = await _branchRepository.GetBySystemAsync(systemId);
                foreach (var branch in branches)
                {
                    foreach (var program in programIds)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        string processing = systemIds.Count == 1
                            ? $"Processing: {branch.Name} - {program.Name} -"
                            : $"Processing: {branch.SystemName} - {branch.Name} -";

                        // clear counters of participants that achieved point values
                        var programCheck = new Dictionary<long, long>();
                        foreach (var pointValue in pointValues)
                        {
                            programCheck.Add(pointValue, 0);
                        }

                        // get users for this program, branch, system
                        criterion.ProgramId = program.Id;
                        criterion.BranchId = branch.Id;
                        criterion.SystemId = systemId;

                        var userIds = await _userRepository
                            .GetUserIdsByBranchProgram(criterion);

                        foreach (var userId in userIds)
                        {
                            count++;
                            if (count % 10 == 0)
                            {
                                UpdateProgress(progress,
                                    Math.Max((int)((count * 100) / reportUserCount), 1),
                                    $"{processing} {count}/{reportUserCount}");
                            }
                            var pointsUntilPeriod = await _userLogRepository
                                .GetEarningsUpToDateAsync(userId,
                                    criterion.StartDate ?? DateTime.MinValue);
                            var pointsDuringPeriod = await _userLogRepository
                                .GetEarningsOverPeriodAsync(userId, criterion);

                            var pointsUntilPeriodEnd = pointsUntilPeriod + pointsDuringPeriod;

                            foreach (var pointValue in pointValues)
                            {
                                if (pointsUntilPeriod < pointValue
                                    && pointValue <= pointsUntilPeriodEnd)
                                {
                                    programCheck[pointValue] += 1;
                                    if(branch.Id == 1 && program.Id == 2 && pointValue == 500)
                                    {
                                        _logger.LogInformation($"User {userId}: {pointsUntilPeriod} < {pointValue} <= {pointsUntilPeriodEnd}");
                                    }
                                }
                            }
                        }
                        // add row
                        row = new List<object>();
                        row.Add(branch.SystemName);
                        row.Add(branch.Name);
                        row.Add(program.Name);
                        row.Add(userIds.Count);
                        foreach (var pointValue in pointValues)
                        {
                            row.Add(programCheck[pointValue]);
                            totalCheck[pointValue] += programCheck[pointValue];
                        }
                        reportData.Add(row.ToArray());

                        totalRegistered += userIds.Count();
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

            report.Data = reportData.ToArray();

            // total row
            row = new List<object>();
            row.Add("Total");
            row.Add(string.Empty);
            row.Add(string.Empty);
            row.Add(totalRegistered);
            foreach (var pointValue in pointValues)
            {
                row.Add(totalCheck[pointValue]);
            }
            report.FooterRow = row.ToArray();
            report.AsOf = _serviceFacade.DateTimeProvider.Now;

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
