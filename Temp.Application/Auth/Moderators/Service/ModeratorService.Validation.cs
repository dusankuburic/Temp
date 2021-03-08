using System.Collections.Generic;
using Temp.Domain.Models;
using Temp.Domain.Models.ModeratorGroups.Exceptions;

namespace Temp.Application.Auth.Moderators.Service
{
    public partial class ModeratorService
    {
        public void ValidateModeratorGroups(IEnumerable<ModeratorGroup> moderatorGroups)
        {
            if (moderatorGroups is null)
            {
                throw new NullModeratorGroupsException();
            }
        }
    }
}