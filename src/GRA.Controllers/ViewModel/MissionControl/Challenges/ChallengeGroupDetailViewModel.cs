using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Controllers.ViewModel.MissionControl.Challenges
{
    public class ChallengeGroupDetailViewModel
    {
        public GRA.Domain.Model.ChallengeGroup ChallengeGroup { get; set; }
        public string ChallengeIds { get; set; }
        public string Action { get; set; }
    }
}
