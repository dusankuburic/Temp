using Temp.Domain.Models;
using Temp.Services.Providers.Models;

namespace Temp.Services.Providers;

public static class IdentityHelpers
{
    public static BaseEntity SetAuditableInfoOnCreate(this BaseEntity entity, CurrentUser currentUser) {
        entity.CreatedBy = currentUser.AppUserId;
        entity.UpdatedBy = currentUser.AppUserId;

        return entity;
    }

    public static BaseEntity SetAuditableInfoOnUpdate(this BaseEntity entity, CurrentUser currentUser) {
        entity.UpdatedBy = currentUser.AppUserId;

        return entity;
    }
}
