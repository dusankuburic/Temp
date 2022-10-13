using System.ComponentModel.DataAnnotations;
using Temp.Database;
using Temp.Domain.Models.Applications;

namespace Temp.Services.Applications.CLI.Command
{
    public class CreateApplication
    {
        private readonly ApplicationDbContext _ctx;

        public CreateApplication(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public async Task<Response> Do(Application application) {

            _ctx.Applications.Add(application);
            await _ctx.SaveChangesAsync();

            return new Response {
                Status = true
            };
        }


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
