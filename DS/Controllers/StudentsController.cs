using BLL.Services;
using DomainModel;
using Interfaces.DTO;
using Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService studentService;
        private readonly ILogger<TeachersController> _logger;
        private readonly UserManager<User> _userManager;
        public StudentsController(IStudentService studentService, ILogger<TeachersController> logger, UserManager<User> userManager)
        {
            this.studentService
                = studentService;
            this._userManager
                 = userManager;
            this._logger = logger;
        }

        // GET: api/<StudentsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<userDTO>>> GetStudents()
        {
            try
            {
                var result = await studentService.GetStudentsListAsync();//_context.Lessons.Include(l => l.Student).Include(t => t.Teacher).Include(q => q.Type).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в методе GetStudents");
                return StatusCode(500);//, "Внутренняя ошибка сервера"
            }
        }
        //// GET: api/<StudentsController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<StudentsController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<StudentsController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<StudentsController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<StudentsController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
