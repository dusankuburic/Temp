﻿using System.Collections.Generic;

namespace Temp.Domain.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Group> Groups { get; set; }
    }
}
