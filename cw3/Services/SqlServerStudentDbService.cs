using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s18996; Integrated Security=True";
        public Enrollment EnrollStudent(EnrollStudentRequest request)
        {
            using(var con = new SqlConnection(ConString))
            using(var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    /*
                        com.CommandText = "exec EnrollStudentSql @IndexNumber, @FirstName, @LastName, @BirthDate, @Studies, @Semester";
                        com.Parameters.AddWithValue("indexnumber", request.IndexNumber);
                        com.Parameters.AddWithValue("firstname", request.FirstName);
                        com.Parameters.AddWithValue("lastname", request.LastName);
                        com.Parameters.AddWithValue("birthdate", request.BirthDate);
                        com.Parameters.AddWithValue("studies", request.Studies);
                        com.Parameters.AddWithValue("semester", request.Semester);

                        com.ExecuteNonQuery();
                    */
                    com.CommandText = "select IdStudies from studies where name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        tran.Rollback();
                        return null;
                    }
                    com.CommandText = "select * from Enrollment where IdStudy=(select IdStudy from studies where name=@name AND semester=1)";
                    com.Parameters.AddWithValue("name", request.Studies);
                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        com.CommandText = 
                            "insert into Enrollment values ((select max(idEnrollment) from enrollment)+1, 1, select idStudy from studies where name=@name AND semester=1, GETDATE())";
                        com.Parameters.AddWithValue("name", request.Studies);
                        com.ExecuteNonQuery();
                    }
                    com.CommandText = "select * from students where indexNumber=@indexNumber";
                    com.Parameters.AddWithValue("indexNumber", request.IndexNumber);
                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        com.CommandText = "insert into Student values (@indexNumber, @FirstName, @lastName, @birthDate, (select idEnrollment from enrollment where idStudy=(select idStudy from studies where name=@name AND semester=1)), GETDATE()";
                        com.Parameters.AddWithValue("indexNumber", request.IndexNumber);
                        com.Parameters.AddWithValue("firstName", request.FirstName);
                        com.Parameters.AddWithValue("lastName", request.LastName);
                        com.Parameters.AddWithValue("birthDate", request.BirthDate);
                        com.Parameters.AddWithValue("name", request.Studies);
                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        tran.Rollback();
                        return null;
                    }

                    com.CommandText = "select * from Enrollment e, Student s where e.IdEnrollment = s.IdEnrollment";
                    var enrollment = new Enrollment();
                    enrollment.Semester = request.Semester;
                    enrollment.IdEnrollment = (int)dr["IdEnrollment"];
                    enrollment.IdStudy = (int)dr["IdStudy"];
                    enrollment.StartDate = (DateTime)dr["StartDate"];
                    var st = new Student();
                    st.enrollment = enrollment;
                    tran.Commit();
                    return st.enrollment;
                    
                }catch(SqlException exc)
                {
                    tran.Rollback();
                }
            }
            return null;
        }

        public Enrollment PromoteStudents(PromoteStudentsRequest request)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    com.CommandText = "exec PromoteStudents @Studies, @Semester";
                    com.Parameters.AddWithValue("studies", request.Studies);
                    com.Parameters.AddWithValue("semester", request.Semester);
                    com.ExecuteNonQuery();
                    tran.Commit();

                    com.CommandText = "select * from Enrollment where IdStudy=(select IdStudy from studies where Name=@studies AND Semester=@semester)";
                    com.Parameters.AddWithValue("studies", request.Studies);
                    com.Parameters.AddWithValue("semester", request.Semester);
                    var dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        var enrollment = new Enrollment();
                        enrollment.Semester = request.Semester;
                        enrollment.IdEnrollment = (int)dr["IdEnrollment"];
                        enrollment.IdStudy = (int)dr["IdStudy"];
                        enrollment.StartDate = (DateTime)dr["StartDate"];
                        return enrollment;
                    }
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                }
            }
            return null;
        }
    }
}
