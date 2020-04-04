using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;

        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult EnrollNewStudent(EnrollStudentRequest req)
        {
            var tmp = _service.EnrollStudent(req);
            int sem = 0;
            if (tmp == null)
                return BadRequest();
            if (sem != 0)
                return Created("Created student on semester: ", sem);
            return BadRequest();
        }

        [HttpPost]
        [Route("promotions")]
        public IActionResult PromoteAllStudents(PromoteStudentsRequest req)
        {
            var tmp = _service.PromoteStudents(req);
            if(tmp != null)
                return Created("Enrollment: ", tmp);
            return BadRequest();
        }

    }
}