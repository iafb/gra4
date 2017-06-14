using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class BundlesDetailViewModel
    {
        public GRA.Domain.Model.DynamicAvatarBundle Bundle { get; set; }
        public string Action { get; set; }
        [Required(ErrorMessage = "Please select items for the bundle")]
        public string ItemsList { get; set; }
        public SelectList Layers { get; set; }
    }
}
