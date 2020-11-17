﻿using System.Collections.Generic;
using Temp.Application.EmploymentStatuses;
using Temp.Application.Empolyees;
using Temp.Application.Workplaces;
using Temp.Database;

namespace Temp.Application.Engagements
{
    public class GetCreateEngagementViewModel : EngagementService
    {
        private readonly ApplicationDbContext _ctx;

        public GetCreateEngagementViewModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
       
        public Response Do(int id) =>
        TryCatch(() => 
        { 
            var response = new Response
            {
                Employee = new GetEmployee(_ctx).Do(id),
                Workplaces = new GetWorkplaces(_ctx).Do(),
                EmploymentStatuses = new GetEmploymentStatuses(_ctx).Do()
            };

            ValidateCreateEngagementViewModel(response);

            return response;
        });
            



        public class Response
        {
            public GetEmployee.EmployeeViewModel Employee;
            public IEnumerable<GetWorkplaces.WorkplacesViewModel> Workplaces;
            public IEnumerable<GetEmploymentStatuses.EmploymentStatusViewModel> EmploymentStatuses;
        }
    }
}