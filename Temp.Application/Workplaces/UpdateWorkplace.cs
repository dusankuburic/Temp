using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Application.Workplaces
{
    public class UpdateWorkplace
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateWorkplace(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Response> Do(Request request)
        {
            var workplace = _ctx.Workplaces.FirstOrDefault(x => x.Id == request.Id);

            workplace.Name = request.Name;

            await _ctx.SaveChangesAsync();

            return new Response
            {
                Id = workplace.Id,
                Name = workplace.Name,
                Message = "Success",
                Status = true
            };
        }


        public class Request
        {
            [Required]
            public int Id {get; set;}

            [MinLength(2)]
            [MaxLength(50)]
            public string Name {get; set;}
        }

        public class Response
        {
            public int  Id {get; set;}
            public string Name {get; set;}
            public string Message {get; set;}
            public bool Status {get; set;}
        }

    }
}
