﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Organizations.Service;
using Temp.Database;

namespace Temp.Application.Organizations
{
    public class GetOrganizations : OrganizationService
    {
        private readonly ApplicationDbContext _ctx;

        public GetOrganizations(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public  Task<IEnumerable<OrganizationViewModel>> Do() =>
        TryCatch(async() =>
        {
            var organizations = await _ctx.Organizations
            .Select(x => new OrganizationViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

            ValidateStorageOrganizations(organizations);

            return organizations;
        });
   
        public class OrganizationViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
