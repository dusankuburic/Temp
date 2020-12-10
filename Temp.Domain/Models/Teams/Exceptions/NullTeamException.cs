using System;

namespace Temp.Domain.Models.Teams.Exceptions
{
    public class NullTeamException : Exception
    {
        public NullTeamException() : base("team is null")
        {

        }
    }
}
