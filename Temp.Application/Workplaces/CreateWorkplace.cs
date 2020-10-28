using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Workplaces
{
    public class CreateWorkplace
    {
        private readonly ApplicationDbContext _ctx;
        public CreateWorkplace(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Response> Do(Request request)
        {
            var workplace = new Workplace
            {
                Name = request.Name
            };

            _ctx.Workplaces.Add(workplace);
            await _ctx.SaveChangesAsync();

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
            public string Name {get; set;}
        }

        public class Response
        {
            public string Message {get; set;}
            public bool Status {get; set;}
        }
    }
}
