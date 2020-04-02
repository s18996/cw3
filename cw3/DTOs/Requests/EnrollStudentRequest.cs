using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required]
        public int IndexNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string Studies { get; set; }
        [Required]
        public int Semester { get; set; }

    }
}
