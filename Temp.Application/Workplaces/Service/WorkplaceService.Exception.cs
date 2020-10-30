using System;
using System.Threading.Tasks;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.Application.Workplaces.Service
{
    public partial class WorkplaceService
    {
        public delegate Task<CreateWorkplace.Response> ReturningWorkplaceFunction();

        public async Task<CreateWorkplace.Response> TryCatch(ReturningWorkplaceFunction returningWorkplaceFunction)
        {
            try
            {
                return await returningWorkplaceFunction();
            }
            catch(NullWorkplaceException nullWorkplaceException)
            {
                throw CreateAndLogValidationException(nullWorkplaceException);
            }
            catch(InvalidWorkplaceException invalidWorkplaceException)
            {
                throw CreateAndLogValidationException(invalidWorkplaceException);
            }
        }

        private WorkplaceValidationException CreateAndLogValidationException(Exception exception)
        {
            var workplaceValidationException = new WorkplaceValidationException(exception);

            return workplaceValidationException;
        }    
    }
}
