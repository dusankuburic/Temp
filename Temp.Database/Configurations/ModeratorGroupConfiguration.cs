using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temp.Domain.Models;

namespace Temp.Database.Configurations;

public class ModeratorGroupConfiguration : IEntityTypeConfiguration<ModeratorGroup>
{
    public void Configure(EntityTypeBuilder<ModeratorGroup> builder) {
        builder.HasKey(x => new { x.ModeratorId, x.GroupId });
    }
}
