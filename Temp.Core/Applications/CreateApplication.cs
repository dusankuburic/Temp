using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Core.Applications.Service;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Core.Applications
{
    public class CreateApplication : ApplicationService
    {
        private readonly ApplicationDbContext _ctx;

        public CreateApplication(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<Response> Do(Request request) =>
        TryCatch(async () => {
            var application = new Application
            {
                UserId = request.UserId,
                TeamId = request.TeamId,
                Content = request.Content,
                Category = request.Category,
                CreatedAt = DateTime.Now
            };

            ValidateApplicationOnCreate(application);

            _ctx.Applications.Add(application);
                await _ctx.SaveChangesAsync();

            return new Response
            {
                Status = true
            };
        });
 

        public class Request
        {
            [Required]
            public int UserId { get; set; }
            [Required]
            public int TeamId { get; set; }
            [Required]
            [MaxLength(600)]
            public string Content { get; set; }
            [Required]
            public string Category { get; set; }

        }

        public class Response
        {
            public bool Status { get; set; }
        }
    }
}
