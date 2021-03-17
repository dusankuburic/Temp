using System;
using Temp.Domain.Models;
using Temp.Domain.Models.Groups.Exceptions;

namespace Temp.Core.Groups.Service
{
    public partial class GroupService
    {
        public void ValidateGroupOnCreate(Group group)
        {
            ValidateGroup(group);
            ValidateGroupString(group);
        }

        public void ValidateGroupOnUpdate(Group group)
        {
            ValidateGroup(group);
            ValidateGroupString(group);
        }

        public void ValidateGroup(Group group)
        {
            if(group is null)
            {
                throw new NullGroupException();
            }
        }

        public void ValidateGetGroupViewModel(GetGroup.GroupViewModel group)
        {
            if(group is null)
            {
                throw new NullGroupException();
            }
        }

        public void ValidateGroupString(Group group)
        {
            switch(group)
            {
                case { } when IsInvalid(group.Name):
                    throw new InvalidGroupException(
                        parameterName: nameof(group.Name),
                        parameterValue: group.Name);
            }
        }


        public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);
    }
}
