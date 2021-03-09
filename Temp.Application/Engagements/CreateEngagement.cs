using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Engagements
{
    public class CreateEngagement : EngagementService
    {
        private readonly ApplicationDbContext _ctx;

        public CreateEngagement(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        

        public Task<Response> Do(Request request) =>
        TryCatch(async () =>
        { 
            var engagement = new Engagement
            {
                EmployeeId = request.EmployeeId,
                WorkplaceId = request.WorkplaceId,
                EmploymentStatusId = request.EmploymentStatusId,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                Salary = request.Salary
            };

            ValidateEngagementOnCreate(engagement);

            _ctx.Engagements.Add(engagement);
            await _ctx.SaveChangesAsync();

            return new Response
            {
                Message = $"Success",
                Status = true
            };
        });
        

       
        public class Request
        {
            [Required]
            public int EmployeeId {get; set;}

            [Required]
            public int WorkplaceId {get; set;}

            [Required]
            public int EmploymentStatusId {get; set;}

            [Required]
            public DateTime DateFrom {get; set;}
            
            [Required]
            public DateTime DateTo {get; set;}
            
            [Required]
            public int Salary { get; set; }
        }


         public class Response
        {
            public string Message {get; set;}
            public bool Status {get; set;}
        }
        
    }
}
