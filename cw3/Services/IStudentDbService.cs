using cw3.DTOs.Requests;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Services
{
    public interface IStudentDbService
    {
        Enrollment EnrollStudent(EnrollStudentRequest request);
        Enrollment PromoteStudents(PromoteStudentsRequest request);
    }
}
