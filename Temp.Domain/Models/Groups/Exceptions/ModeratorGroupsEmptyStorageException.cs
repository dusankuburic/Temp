using System;

namespace Temp.Domain.Models.Groups.Exceptions
{
    public class ModeratorGroupsEmptyStorageException : Exception
    {
        public ModeratorGroupsEmptyStorageException() :
            base("Moderator doesn't have any assigned groups") {

        }
    }
}