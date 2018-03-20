﻿using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class AvatarViewModel
    {
        public ICollection<AvatarLayer> Layers { get; set; }
        public IEnumerable<int> GroupIds { get; set; }

        public int DefaultLayer { get; set; }
        public string ImagePath { get; set; }
        public string AvatarPiecesJson { get; set; }
    }
}
