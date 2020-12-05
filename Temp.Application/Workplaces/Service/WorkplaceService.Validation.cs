using System;
using System.Collections.Generic;
using System.Linq;
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

        public void ValidateWorkplaceOnUpdate(Workplace workplace)
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

        public void ValidateGetWorkplaceViewModel(GetWorkplace.WorkplaceViewModel workplace)
        {
            if(workplace is null)
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

        public void ValidateStorageWorkplaces(IEnumerable<GetWorkplaces.WorkplacesViewModel> storageWorkplaces)
        {
            if(storageWorkplaces.Count() == 0)
            {
                throw new WorkplaceEmptyStorageException();
            }
        }

        public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);        
    }
}
