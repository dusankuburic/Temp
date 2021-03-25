using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Temp.Core.Applications.Service;
using Temp.Database;

namespace Temp.Core.Applications
{
    public class GetApplication : ApplicationService
    {
        private readonly ApplicationDbContext _ctx;

        public GetApplication(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<ApplicationViewModel> Do(int id) =>
            TryCatch(async () =>
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

                ValidateGetApplicationViewModel(application);

                return application;
            });

        public class ApplicationViewModel
        {
            public int Id { get; set; }
            public string Category { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
