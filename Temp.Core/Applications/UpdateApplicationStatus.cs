using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Core.Applications
{
    public class UpdateApplicationStatus
    {
        private readonly ApplicationDbContext _ctx;


        public UpdateApplicationStatus(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Response> Do(int id, Request request)
        {

            var application = await _ctx.Applications
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            //Validate application

            application.ModeratorId = request.ModeratorId;
            application.Status = true;

            await _ctx.SaveChangesAsync();

            return new Response
            {
                Id = application.Id,
                Status = true
            };
        }
             
        public class Request
        {
            public int ModeratorId { get; set; }
        }

        public class Response
        {
            public int Id { get; set; }
            public bool Status { get; set; }
        }
    }
}
