using System;
using GRA.Domain.Model;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Generic;

namespace GRA.Domain.Report.Abstract
{
    public abstract class BaseReport
    {
        protected readonly ILogger _logger;
        protected readonly ServiceFacade.Report _serviceFacade;
        private Stopwatch _timer;

        protected StoredReportSet ReportSet { get; set; }

        public BaseReport(ILogger logger,
            ServiceFacade.Report serviceFacade)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceFacade = serviceFacade
                ?? throw new ArgumentNullException(nameof(serviceFacade));
            ReportSet = new StoredReportSet();
            ReportSet.Reports = new List<StoredReport>();
        }

        protected async Task<ReportRequest> StartRequestAsync(ReportRequest request)
        {
            if(_timer == null)
            {
                _timer = new Stopwatch();
            }
            _timer.Start();

            request.Started = _serviceFacade.DateTimeProvider.Now;
            request.Finished = null;
            request.Success = null;
            request.ResultJson = null;
            request.InstanceName = null;
            request.Name = request.Name;
            return await _serviceFacade.ReportRequestRepository.UpdateSaveNoAuditAsync(request);
        }

        protected double StopTimer()
        {
            _timer.Stop();
            double elapsed = _timer.Elapsed.TotalSeconds;
            _timer = null;
            return elapsed;
        }

        public abstract Task ExecuteAsync(ReportRequest request,
            CancellationToken token,
            IProgress<OperationStatus> progress = null);

        protected void UpdateProgress(IProgress<OperationStatus> progress, int percentComplete)
        {
            if (progress != null)
            {
                progress.Report(new OperationStatus
                {
                    PercentComplete = percentComplete
                });
            }
        }

        protected void UpdateProgress(IProgress<OperationStatus> progress, string message)
        {
            if (progress != null)
            {
                progress.Report(new OperationStatus
                {
                    Status = message
                });
            }
        }
    }
}
