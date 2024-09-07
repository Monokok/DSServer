//using Microsoft.AspNetCore.Cors; //CORS
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Reflection.Metadata;
//using DomainModel;
//using DAL;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace DS.Controllers
//{
//    [Route("api/[controller]")]
//    [EnableCors] //CORS
//    [ApiController]
//    public class CoursesController : ControllerBase
//    {
//        private readonly DrivingSchoolContext _context;
//        public CoursesController(DrivingSchoolContext context)
//        {
//            _context = context;
//            if (!_context.Courses.Any())
//            {
//                //_context.Courses.Add(new Course
//                //{
//                //    StartTime = DateTime.Now,
//                //    EndTime = DateTime.Now.AddMonths(2),
//                //    DrivingHours = 45,
//                //    LectureHours = 30,
//                //    TeacherId = "1",
//                //    CathegoryId = 1,
//                //    Id = 0
//                //});
//                //_context.SaveChanges();
//            }
//        }

//        // GET: api/Courses
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
//        {
//            return await _context.Courses/*.Include(p => p.Groups)*/.ToListAsync();
//        }
//        // GET: api/Courses/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Course>> GetCourse(int id)
//        {
//            var сourse = await _context.Courses.FindAsync(id);
//            if (сourse == null)
//            {
//                return NotFound();
//            }
//            return сourse;
//        }

//        //// GET: api/<CoursesController>
//        //[HttpGet]
//        //public IEnumerable<string> Get()
//        //{
//        //    return new string[] { "value1", "value2" };
//        //}

//        //// GET api/<CoursesController>/5
//        //[HttpGet("{id}")]
//        //public string Get(int id)
//        //{
//        //    return "value";
//        //}

//        // POST api/<CoursesController>
//        [HttpPost]
//        public void Post([FromBody] string value)
//        {
//        }

//        // PUT api/<CoursesController>/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody] string value)
//        {
//        }

//        // DELETE api/<CoursesController>/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }
//    }
//}
