using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Applications.Service;
using Temp.Database;

namespace Temp.Core.Applications
{
    public class UpdateApplicationStatus : ApplicationService
    {
        private readonly ApplicationDbContext _ctx;


        public UpdateApplicationStatus(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public Task<Response> Do(int id, Request request) =>
        TryCatch(async () => {

            var application = await _ctx.Applications
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

            ValidateApplication(application);

            application.ModeratorId = request.ModeratorId;
            application.Status = true;
            application.StatusUpdatedAt = DateTime.Now;

            await _ctx.SaveChangesAsync();

            return new Response {
                Id = application.Id,
                Status = true
            };
        });

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