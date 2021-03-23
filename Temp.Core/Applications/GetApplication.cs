using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Core.Applications
{
    public class GetApplication
    {
        private readonly ApplicationDbContext _ctx;

        public GetApplication(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ApplicationViewModel> Do(int id)
        {
            var application = await _ctx.Applications
                .Where(x => x.Id == id)
                .Select(x => new ApplicationViewModel
                {
                    Id = x.Id,
                    Category = x.Category,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            return application;
        }

        public class ApplicationViewModel
        {
            public int Id { get; set; }
            public string Category { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
