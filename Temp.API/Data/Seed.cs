
using Newtonsoft.Json;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.API.Data;

public class Seed
{
    public static void SeedOrganizations(ApplicationDbContext ctx) {
        if (!ctx.Organizations.Any()) {
            var organizationData = File.ReadAllText("Data/OrganizationSeedData.json");
            var organizations = JsonConvert.DeserializeObject<List<Organization>>(organizationData);

            organizations.ForEach(x => {
                x.CreatedAt = DateTime.UtcNow;
                x.CreatedBy = "System";
                x.UpdatedAt = DateTime.UtcNow;
                x.UpdatedBy = "System";
            });

            ctx.Organizations.AddRange(organizations);

            ctx.SaveChanges();
        }
    }

    public static void SeedGroups(ApplicationDbContext ctx) {
        if (!ctx.Groups.Any()) {
            var groupData = File.ReadAllText("Data/GroupSeedData.json");
            var groups = JsonConvert.DeserializeObject<List<Group>>(groupData);

            groups.ForEach(x => {
                x.CreatedAt = DateTime.UtcNow;
                x.CreatedBy = "System";
                x.UpdatedAt = DateTime.UtcNow;
                x.UpdatedBy = "System";
            });

            ctx.Groups.AddRange(groups);

            ctx.SaveChanges();
        }
    }

    public static void SeedTeams(ApplicationDbContext ctx) {
        if (!ctx.Teams.Any()) {
            var teamData = File.ReadAllText("Data/TeamSeedData.json");
            var teams = JsonConvert.DeserializeObject<List<Team>>(teamData);

            teams.ForEach(x => {
                x.CreatedAt = DateTime.UtcNow;
                x.CreatedBy = "System";
                x.UpdatedAt = DateTime.UtcNow;
                x.UpdatedBy = "System";
            });

            ctx.Teams.AddRange(teams);

            ctx.SaveChanges();
        }
    }

    public static void SeedEmploymentStatuses(ApplicationDbContext ctx) {
        if (!ctx.EmploymentStatuses.Any()) {
            var employmentStatusData = File.ReadAllText("Data/EmploymentStatusSeedData.json");
            var employmentStatuses = JsonConvert.DeserializeObject<List<EmploymentStatus>>(employmentStatusData);

            employmentStatuses.ForEach(x => {
                x.CreatedAt = DateTime.UtcNow;
                x.CreatedBy = "System";
                x.UpdatedAt = DateTime.UtcNow;
                x.UpdatedBy = "System";
            });

            ctx.EmploymentStatuses.AddRange(employmentStatuses);

            ctx.SaveChanges();
        }
    }

    public static void SeedWorkplaces(ApplicationDbContext ctx) {
        if (!ctx.Workplaces.Any()) {
            var workplaceData = File.ReadAllText("Data/WorkplaceSeedData.json");
            var workplaces = JsonConvert.DeserializeObject<List<Workplace>>(workplaceData);

            workplaces.ForEach(x => {
                x.CreatedAt = DateTime.UtcNow;
                x.CreatedBy = "System";
                x.UpdatedAt = DateTime.UtcNow;
                x.UpdatedBy = "System";
            });

            ctx.Workplaces.AddRange(workplaces);

            ctx.SaveChanges();
        }
    }

    public static void SeedEmployees(ApplicationDbContext ctx) {
        if (!ctx.Employees.Any()) {
            var employeesData = File.ReadAllText("Data/EmployeeSeedData.json");
            var employees = JsonConvert.DeserializeObject<List<Employee>>(employeesData);

            employees.ForEach(x => {
                x.CreatedAt = DateTime.UtcNow;
                x.CreatedBy = "System";
                x.UpdatedAt = DateTime.UtcNow;
                x.UpdatedBy = "System";
                if (string.IsNullOrWhiteSpace(x.AppUserId)) {
                    x.AppUserId = Guid.NewGuid().ToString();
                }
            });

            ctx.Employees.AddRange(employees);

            ctx.SaveChanges();
        }
    }
}