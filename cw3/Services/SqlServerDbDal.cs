using cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Services
{
    public class SqlServerDbDal : IStudentDal
    {
        public IEnumerable<Student> GetStudents()
        {
            //...sql con
            return null;
        }
    }
}
