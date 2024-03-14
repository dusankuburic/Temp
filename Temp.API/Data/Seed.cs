using System.Security.Cryptography;
using Newtonsoft.Json;
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
                x.UpdatedAt = DateTime.UtcNow;
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
                x.UpdatedAt = DateTime.UtcNow;
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
                x.UpdatedAt = DateTime.UtcNow;
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
                x.UpdatedAt = DateTime.UtcNow;
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
                x.UpdatedAt = DateTime.UtcNow;
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
                x.UpdatedAt = DateTime.UtcNow;
            });

            ctx.Employees.AddRange(employees);

            ctx.SaveChanges();
        }
    }

    //public static void SeedAdmins(ApplicationDbContext ctx) {
    //    if (!ctx..Any()) {
    //        var adminsData = File.ReadAllText("Data/AdminSeedData.json");
    //        var admins = JsonConvert.DeserializeObject<List<Admin>>(adminsData);

    //        foreach (var admin in admins) {
    //            byte[] passwordHash, passwordSalt;
    //            CreatePasswordHash($"{admin.Username}123", out passwordHash, out passwordSalt);

    //            admin.PasswordHash = passwordHash;
    //            admin.PasswordSalt = passwordSalt;
    //            admin.IsActive = true;
    //        }


    //        ctx.Admins.AddRange(admins);

    //        ctx.SaveChanges();
    //    }
    //}

    //public static void SeedUsers(ApplicationDbContext ctx) {
    //    if (!ctx.Users.Any()) {
    //        var usersData = File.ReadAllText("Data/UserSeedData.json");
    //        var users = JsonConvert.DeserializeObject<List<User>>(usersData);

    //        foreach (var user in users) {
    //            byte[] passwordHash, passwordSalt;
    //            CreatePasswordHash($"{user.Username}123", out passwordHash, out passwordSalt);

    //            user.PasswordHash = passwordHash;
    //            user.PasswordSalt = passwordSalt;
    //            user.IsActive = true;
    //        }

    //        ctx.Users.AddRange(users);

    //        ctx.SaveChanges();
    //    }
    //}

    //public static void SeedModerators(ApplicationDbContext ctx) {
    //    if (!ctx.Moderators.Any()) {
    //        var moderatorsData = File.ReadAllText("Data/ModeratorSeedData.json");
    //        var moderators = JsonConvert.DeserializeObject<List<Moderator>>(moderatorsData);

    //        foreach (var moderator in moderators) {
    //            byte[] passwordHash, passwordSalt;
    //            CreatePasswordHash($"{moderator.Username}123", out passwordHash, out passwordSalt);

    //            moderator.PasswordHash = passwordHash;
    //            moderator.PasswordSalt = passwordSalt;
    //            moderator.IsActive = true;
    //        }

    //        ctx.Moderators.AddRange(moderators);

    //        ctx.SaveChanges();
    //    }
    //}


    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
        using (var hmac = new HMACSHA512()) {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
