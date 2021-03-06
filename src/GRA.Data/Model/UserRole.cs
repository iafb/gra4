﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserRole
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
