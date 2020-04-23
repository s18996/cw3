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
        Student GetStudent(string index);
        Student GetStudentWithToken(string refreshToken);
        IEnumerable<Student> GetStudents();
        string getSaltFromDB(string index);
        bool IsLoginCorrect(LoginRequest request);
        void AddTokenToDB(string token, string index);
        bool IsRefTokenInDB(string token);
        void UpdateTokenInDB(string oldToken, string newToken);
    }
}
