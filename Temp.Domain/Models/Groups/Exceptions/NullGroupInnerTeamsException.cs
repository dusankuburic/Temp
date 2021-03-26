using System;

namespace Temp.Domain.Models.Groups.Exceptions
{
    public class NullGroupInnerTeamsException: Exception
    {

        public NullGroupInnerTeamsException():
            base("inner team is null")
        {

        }
    }
}
