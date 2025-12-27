using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temp.Domain.Models;

namespace Temp.Database.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder) {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.GroupId)
            .IsRequired();


        builder.HasIndex(x => x.GroupId)
            .HasDatabaseName("IX_Teams_GroupId");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Teams_IsActive");

        builder.HasIndex(x => new { x.GroupId, x.IsActive })
            .HasDatabaseName("IX_Teams_GroupId_IsActive");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Teams_Name");
    }
}