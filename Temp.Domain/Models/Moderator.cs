﻿
using System.Collections.Generic;

namespace Temp.Domain.Models
{
    public class Moderator
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        
        public ICollection<ModeratorGroup> ModeratorGroups { get; set; }
    }
}