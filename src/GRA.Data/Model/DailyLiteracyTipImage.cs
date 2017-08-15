using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GRA.Data.Model
{
    public class DailyLiteracyTipImage : Abstract.BaseDbEntity
    {
        public int DailyLiteracyTipId { get; set; }
        public DailyLiteracyTip DailyLiteracyTip { get; set; }
        
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
