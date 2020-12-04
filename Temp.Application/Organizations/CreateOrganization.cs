using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Organizations
{
    public class CreateOrganization
    {
        private readonly ApplicationDbContext _ctx;

        public CreateOrganization(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private async Task<bool> OrganizationExists(string name)
        {
            if(await _ctx.Organizations.AnyAsync(x => x.Name == name))
            {
                return true;
            }
            return false;
        }

        public async Task<Response> Do(Request request)
        {
            var organizationExists = await OrganizationExists(request.Name);

            if (organizationExists)
            {
                return new Response
                {
                    Message = $"Error {request.Name} already exists",
                    Status = false
                };
            }

            var organization = new Organization
            {
                Name = request.Name
            };

            return new Response
            {
                Message = $"Success {request.Name} is added",
                Status = true
            };
        }

        public class Request
        {
            [Required]
            [MinLength(2)]
            [MaxLength(50)]
            public string Name { get; set; }
        }

        public class Response
        {
            public string Message { get; set; }
            public bool Status { get; set; }
        }
    }
}
