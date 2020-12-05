using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Temp.Domain.Models.Organizations.Exceptions;

namespace Temp.Application.Organizations.Service
{
    public partial class OrganizationService
    {
        public delegate Task<CreateOrganization.Response> ReturningCreateOrganizationFunction();
        public delegate IEnumerable<GetOrganizations.OrganizationViewModel> ReturningGetOrganizationsFunction();
        public delegate GetOrganization.OrganizationViewModel ReturningGetOrganizationFunction();
        public delegate Task<UpdateOrganization.Response> ReturningUpdateOrganizationFunction();

        public async Task<CreateOrganization.Response> TryCatch(ReturningCreateOrganizationFunction returningCreateOrganizationFunction)
        {
            try
            {
                return await returningCreateOrganizationFunction();
            }
            catch(NullOrganizationException nullOrganizationException)
            {
                throw CreateAndLogValidationException(nullOrganizationException);
            }
            catch(InvalidOrganizationException invalidOrganizationException)
            {
                throw CreateAndLogValidationException(invalidOrganizationException);
            }
            catch (SqlException sqlExcepton)
            {
                throw CreateAndLogCriticalDependencyException(sqlExcepton);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public IEnumerable<GetOrganizations.OrganizationViewModel> TryCatch(ReturningGetOrganizationsFunction returningGetOrganizationsFunction)
        {
            try
            {
                return returningGetOrganizationsFunction();
            }
            catch(OrganizationEmptyStorageException organizationEmptyStorageException)
            {
                throw CreateAndLogValidationException(organizationEmptyStorageException);
            }
            catch (SqlException sqlExcepton)
            {
                throw CreateAndLogCriticalDependencyException(sqlExcepton);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public GetOrganization.OrganizationViewModel TryCatch(ReturningGetOrganizationFunction returningGetOrganizationFunction)
        {
            try
            {
                return returningGetOrganizationFunction();
            }
            catch (NullOrganizationException nullOrganizationException)
            {
                throw CreateAndLogServiceException(nullOrganizationException);
            }
            catch (SqlException sqlExcepton)
            {
                throw CreateAndLogCriticalDependencyException(sqlExcepton);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }
          
        public async Task<UpdateOrganization.Response> TryCatch(ReturningUpdateOrganizationFunction returningUpdateOrganizationFunction)
        {
            try
            {
                return await returningUpdateOrganizationFunction();
            }
            catch(NullOrganizationException nullOrganizationException)
            {
                throw CreateAndLogValidationException(nullOrganizationException);
            }
            catch(InvalidOrganizationException invalidOrganizationException)
            {
                throw CreateAndLogValidationException(invalidOrganizationException);
            }
            catch (SqlException sqlExcepton)
            {
                throw CreateAndLogCriticalDependencyException(sqlExcepton);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }


        private OrganizationValidationException CreateAndLogValidationException(Exception exception)
        {
            var organizationValidationException = new OrganizationValidationException(exception);
            //LOG
            return organizationValidationException;
        }

        private OrganizationServiceException CreateAndLogServiceException(Exception exception)
        {
            var organizationServiceException = new OrganizationServiceException(exception);
            //LOG
            return organizationServiceException;
        }
    
        private OrganizationDependencyException CreateAndLogCriticalDependencyException(Exception exception)
        {
            var organizationDependencyException = new OrganizationDependencyException(exception);
            //LOG
            return organizationDependencyException;
        }

    }
}
