﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Challenge : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsValid { get; set; }


        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [DisplayName("Points Awarded")]
        [Range(1, int.MaxValue, ErrorMessage = "The minimum points that can be awarded is {1}")]
        public int? PointsAwarded { get; set; }

        [Required]
        [DisplayName("Tasks To Complete")]
        [Range(1, int.MaxValue, ErrorMessage = "The minimum tasks required to complete is {1}")]
        public int? TasksToComplete { get; set; }

        public IEnumerable<ChallengeTask> Tasks { get; set; }
    }
}