using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Temp.Database;

namespace Temp.Application.EmploymentStatuses
{
    public class UpdateEmploymentStatus
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateEmploymentStatus(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }


        public async Task<Response> Do(Request request)
        {
            var employmentStatus = _ctx.EmploymentStatuses.FirstOrDefault(x => x.Id == request.Id);
            
            employmentStatus.Name = request.Name;

            await _ctx.SaveChangesAsync();

            return new Response
            {
                Id = employmentStatus.Id,
                Name = employmentStatus.Name,
                Message = "Success",
                Status = true
            };
        }



        public class Request
        {
            [Required]
            public int Id {get; set;}

            [Required]
            [MaxLength(30)]
            public string Name {get; set;}
        }

        public class Response
        {
            public int Id {get; set;}
            public string Name {get; set;}
            public string Message {get; set;}

            public bool Status {get; set;}
        }
    }
}
