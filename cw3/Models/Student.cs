using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Models
{
    public class Student
    {
        public int IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Enrollment enrollment { get; set; }

    }
}
