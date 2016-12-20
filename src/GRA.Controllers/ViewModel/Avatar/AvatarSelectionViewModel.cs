using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class AvatarSelectionViewModel
    {
        public GRA.Domain.Model.StaticAvatar Avatar { get; set; }
        public int PreviousAvatarId { get; set; }
        public int NextAvatarId { get; set;}
    }
}
