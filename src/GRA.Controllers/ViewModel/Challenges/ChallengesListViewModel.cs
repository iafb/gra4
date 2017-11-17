using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Challenges
{
    public class ChallengesListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Challenge> Challenges { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public string Categories { get; set; }
        public bool IsActive { get; set; }

        public SelectList CategoryList { get; set; }
        public IEnumerable<int> CategoryIds { get; set; }
    }
}
