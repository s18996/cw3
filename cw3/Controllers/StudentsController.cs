using cw3.DTOs.Requests;
using cw3.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        public StudentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private const string ConString = "Data Source=db-mssql;Initial Catalog=s18996; Integrated Security=True";

        private IStudentDbService _dbService;
        public StudentsController(IStudentDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var tmp = _dbService.GetStudents();
            if (tmp == null)
                return BadRequest();
            return Ok(tmp);
        }

        [HttpPost("log")]
        public IActionResult Login(LoginRequest request)
        {
            request.Password = CreateEncodedPassword(request.Login, _dbService.getSaltFromDB(request.Login));
            if (!_dbService.IsLoginCorrect(request))
                return NotFound("No user found with this login and password");
            var stud = _dbService.GetStudent(request.Login);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, stud.Index),
                new Claim(ClaimTypes.Name, stud.FirstName),
                new Claim(ClaimTypes.Role, "employee") // test
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );
            _dbService.AddTokenToDB(token.ToString(), request.Login);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
        }

        [HttpPost("refresh-token/{refreshToken}")]
        public IActionResult RefreshToken(string refreshToken)
        {
            if(!_dbService.IsRefTokenInDB(refreshToken))
                return NotFound("No refToken in db");

            var newRefreshToken = Guid.NewGuid();
            var stud = _dbService.GetStudentWithToken(refreshToken);
            _dbService.UpdateTokenInDB(refreshToken, newRefreshToken.ToString());
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, stud.Index),
                new Claim(ClaimTypes.Name, stud.FirstName),
                new Claim(ClaimTypes.Role, "employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = newRefreshToken
            });
        }

        public static string CreateEncodedPassword(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(valueBytes);
        }

        /*
        [HttpGet]
        public IActionResult GetStudents([FromServices]IStudentDal dbService)
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = (int)dr["IndexNumber"];
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    //st.BirthDate = dr["BirthDate"].ToString();
                    //st.IdEnrollment = dr["IdEnrollment"].ToString();
                    list.Add(st);
                }

            }
            
            return Ok(list);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student where indexnumber=@index";
                com.Parameters.AddWithValue("index", indexNumber);

                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = (int)dr["IndexNumber"];
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    //st.BirthDate = dr["BirthDate"].ToString();
                    //st.IdEnrollment = dr["IdEnrollment"].ToString();
                    return Ok(st);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = new Random().Next(1, 20000);
            return Ok(student);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id)
        {
            return Ok("Aktualizacja dokończona");
        }
        */
    }
}