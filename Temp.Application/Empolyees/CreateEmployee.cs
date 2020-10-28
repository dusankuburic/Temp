using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel.DataAnnotations;
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
            try
            {
                //var empolyee = new Employee
                //{
                //    FirstName = request.FirstName,
                //    LastName = request.LastName
                //};
                Employee empolyee = null;



                _ctx.Employees.Add(empolyee);
                await _ctx.SaveChangesAsync();

                return new Response
                {
                    Message = $"Success {empolyee.FirstName} {empolyee.LastName} is added",
                    Status = true
                };

            }
            catch(Exception ex)
            {
                return new Response
                {
                    Message = $"{ex}",
                    Status = false

                };
            }

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
