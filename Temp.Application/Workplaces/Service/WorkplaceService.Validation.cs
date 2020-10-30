using System;
using Temp.Domain.Models;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.Application.Workplaces.Service
{
    public partial class WorkplaceService
    {

        public void ValidateWorkplaceOnCreate(Workplace workplace)
        {
            ValidateWorkplace(workplace);
            ValidateWorkplaceStrings(workplace);
        }

        public void ValidateWorkplace(Workplace workplace)
        {
            if (workplace is null)
            {
                throw new NullWorkplaceException();
            }
        }

        public void ValidateWorkplaceStrings(Workplace workplace)
        {
            switch(workplace)
            {
                case { } when IsInvalid(workplace.Name):
                    throw new InvalidWorkplaceException(
                        parameterName: nameof(workplace.Name),
                        parameterValue: workplace.Name);
            }
        }
      
        public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);        
    }
}
