using System;
using System.Threading.Tasks;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;

namespace Temp.Application.EmploymentStatuses.Service
{
    public partial class EmploymentStatusService
    {
        public delegate Task<CreateEmploymentStatus.Response> ReturningEmploymentStatusFunction();

        public async Task<CreateEmploymentStatus.Response> TryCatch(ReturningEmploymentStatusFunction returningEmploymentStatusFunction)
        {
            try
            {
                return await returningEmploymentStatusFunction();
            }
            catch(NullEmploymentStatusException nullEmploymentStatusException)
            {
                throw CreateAndLogValidationException(nullEmploymentStatusException);
            }
            catch(InvalidEmploymentStatusException invalidEmploymentStatusException)
            {
                throw CreateAndLogValidationException(invalidEmploymentStatusException);
            }         
        }

        private EmploymentStatusValidationException CreateAndLogValidationException(Exception exception)
        {
            var employmentStatusValidationException = new EmploymentStatusValidationException(exception);

            return employmentStatusValidationException;
        }
    }
}
