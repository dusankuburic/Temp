using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Empolyees
{
    public class CreateEmployee
    {
        private readonly ApplicationDbContext _ctx;

        public CreateEmployee(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }



        public async Task<Response> Do(Request request)
        {
            var empolyee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            _ctx.Employees.Add(empolyee);

            await _ctx.SaveChangesAsync();

            return new Response
            {
                Message = $"Success {empolyee.FirstName} {empolyee.LastName} is added",
                Status = true
            };
        }


        public class Request
        {
            [Required]
            [MaxLength(30)]
            public string FirstName {get; set;}
            [Required]
            [MaxLength(30)]
            public string LastName {get; set;}
        }

        public class Response
        {
            public string Message {get; set;}
            public bool Status {get; set;}
        }

    }
}
