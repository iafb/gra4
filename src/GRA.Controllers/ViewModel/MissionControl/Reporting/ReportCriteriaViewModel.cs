using System;
using System.Collections.Generic;
using System.ComponentModel;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportCriteriaViewModel
    {
        public int ReportId { get; set; }
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        [DisplayName("Select a system")]
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }
        public SelectList SystemList { get; set; }

        [DisplayName("Badges to Report")]
        public ICollection<TriggerRequirement> ReportBadges { get; set; }
        public string BadgeRequiredList { get; set; }
        public string ChallengeRequiredList { get; set; }
    }
}
