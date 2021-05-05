using System.Collections.Generic;
using System.Linq;
using Temp.Database;
using System.IO;
using Newtonsoft.Json;
using Temp.Domain.Models;

namespace Temp.API.Data
{
    public class Seed
    {
        public static void SeedOrganizations(ApplicationDbContext ctx)
        {
            if(!ctx.Organizations.Any())
            {
                var organizationData = File.ReadAllText("Data/OrganizationSeedData.json");
                var organizations = JsonConvert.DeserializeObject<List<Organization>>(organizationData);

                ctx.Organizations.AddRange(organizations);

                ctx.SaveChanges();
            }
        }

        public static void SeedGroups(ApplicationDbContext ctx)
        {
            if(!ctx.Groups.Any())
            {
                var groupData = File.ReadAllText("Data/GroupSeedData.json");
                var groups = JsonConvert.DeserializeObject<List<Group>>(groupData);

                ctx.Groups.AddRange(groups);

                ctx.SaveChanges();
            }
        }

        public static void SeedTeams(ApplicationDbContext ctx)
        {
            if(!ctx.Teams.Any())
            {
                var teamData = File.ReadAllText("Data/TeamSeedData.json");
                var teams = JsonConvert.DeserializeObject<List<Team>>(teamData);

                ctx.Teams.AddRange(teams);

                ctx.SaveChanges();
            }
        }

        public static void SeedEmploymentStatuses(ApplicationDbContext ctx)
        {
            if(!ctx.EmploymentStatuses.Any())
            {
                var employmentStatusData = File.ReadAllText("Data/EmploymentStatusSeedData.json");
                var employmentStatuses = JsonConvert.DeserializeObject<List<EmploymentStatus>>(employmentStatusData);

                ctx.EmploymentStatuses.AddRange(employmentStatuses);

                ctx.SaveChanges();
            }
        }

        public static void SeedWorkplaces(ApplicationDbContext ctx)
        {
            if(!ctx.Workplaces.Any())
            {
                var workplaceData = File.ReadAllText("Data/WorkplaceSeedData.json");
                var workplaces = JsonConvert.DeserializeObject<List<Workplace>>(workplaceData);

                ctx.Workplaces.AddRange(workplaces);

                ctx.SaveChanges();
            }
        }


    }
}
