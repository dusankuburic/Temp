﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Engagements
{
    public class CreateEngagement
    {
        private readonly ApplicationDbContext _ctx;

        public CreateEngagement(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        

        public async Task<Response> Do(Request request)
        {
            var engagement = new Engagement
            {
                EmployeeId = request.EmployeeId,
                WorkplaceId = request.WorkplaceId,
                EmploymentStatusId = request.EmploymentStatusId,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo
            };


            //ValidateEngagementOnCreate(engagement);

            _ctx.Engagements.Add(engagement);
            await _ctx.SaveChangesAsync();

            return new Response
            {
                Message = $"Success",
                Status = true
            };
        }
        

        public class Request
        {
            [Required]
            public int EmployeeId {get; set;}

            [Required]
            public int WorkplaceId {get; set;}

            [Required]
            public int EmploymentStatusId {get; set;}

            [Required]
            public DateTime DateFrom {get; set;}
            
            [Required]
            public DateTime DateTo {get; set;}
        }


         public class Response
        {
            public string Message {get; set;}
            public bool Status {get; set;}
        }
        
    }
}
