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
    public class TeachersController : ControllerBase
    {
        private readonly IStudentService studentService;
        private readonly ILogger<TeachersController> _logger;
        private readonly UserManager<User> _userManager;

        public TeachersController(IStudentService studentService, ILogger<TeachersController> logger, UserManager<User> userManager)
        {
            this.studentService = studentService;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: api/<TeachersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<userDTO>>> GetTeachers()
        {
            try
            {
                var result = await studentService.GetTeachersListAsync();//_context.Lessons.Include(l => l.Student).Include(t => t.Teacher).Include(q => q.Type).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в методе GetTeachers");
                return StatusCode(500);//, "Внутренняя ошибка сервера"
            }
        }

    }
}
