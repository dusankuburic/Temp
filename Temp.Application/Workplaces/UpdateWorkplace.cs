using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Workplaces.Service;
using Temp.Database;

namespace Temp.Application.Workplaces
{
    public class UpdateWorkplace : WorkplaceService
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateWorkplace(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private async Task<bool> WorkplaceExists(string name)
        {
            if (await _ctx.Workplaces.AnyAsync(x => x.Name == name))
            {
                return true;
            }
            return false;
        }

        public Task<Response> Do(int id, Request request) =>
        TryCatch(async () => 
        {
            var workplace = _ctx.Workplaces.FirstOrDefault(x => x.Id == id);

            if(workplace.Name.Equals(request.Name))
            {
                return new Response
                {
                    Id = workplace.Id,
                    Name = workplace.Name,
                    Message = "Workplace name is same",
                    Status = true
                };
            }

            var workplaceExists = await WorkplaceExists(request.Name);

            if(workplaceExists)
            {
                return new Response
                {
                    Id = workplace.Id,
                    Name = workplace.Name,
                    Message = $"Workplace already exists with {request.Name} name",
                    Status = false
                };
            }

            workplace.Name = request.Name;

            ValidateWorkplaceOnUpdate(workplace);

            await _ctx.SaveChangesAsync();

            return new Response
            {
                Id = workplace.Id,
                Name = workplace.Name,
                Message = "Success",
                Status = true
            };
        });

        public class Request
        {

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
