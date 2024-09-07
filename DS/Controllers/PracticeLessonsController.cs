using DAL;
using DomainModel;
using Interfaces.DTO;
using Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Metadata;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DS.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class PracticeLessonsController : ControllerBase
    {
        //private readonly DrivingSchoolContext _context;
        private readonly IStudentService studentService;
        private readonly ILogger<PracticeLessonsController> _logger;

        public PracticeLessonsController(IStudentService studentService, ILogger<PracticeLessonsController> logger)
        {
            this.studentService = studentService;
            _logger = logger;
            //_logger.LogWarning("Запуск LessonsController!");
        }

        // GET api/<LessonsController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "student, teacher" )]

        public async Task<ActionResult<IEnumerable<practiceDTO>>> GetPracticeLessons(string id)
        {
            try
            {
                var lsn = await studentService.GetAllMyLessons(id);
                if (lsn == null)
                {
                    return NotFound();
                }
                else
                    return lsn;
            }
            catch (Exception ex)
            {
                //здесь логирование ошибки
                _logger.LogError(ex, "Ошибка при попытке получить список занятий");
                return StatusCode(500); // Возвращаем код состояния 500 в случае ошибки сервера
            }
        }


        // GET api/<LessonsController>/5
        [HttpGet("{teacherId}/{DayMonthYear}")]
        [Authorize(Roles = "user, teacher")]

        public ActionResult<IEnumerable<string>> GetTimesForLessons(string teacherId, DateTime DayMonthYear)
        {
            try
            {
                var Times = studentService.GetAvailableHours(teacherId, DayMonthYear);
                List<string> times = new List<string>();
                foreach (var item in Times)
                {
                    times.Add(item.ToShortTimeString());
                }
                return times;
            }
            catch (Exception ex)
            {
                // Логируем исключение
                _logger.LogError(ex, "Ошибка при получении доступных вариантов времени для занятия");

                // Возвращаем ошибку клиенту
                return StatusCode(500, "Ошибка при получении доступных времен для занятий");
            }
        }

        // POST api/<LessonsController>
        [HttpPost]
        [Authorize(Roles = "user, teacher")]
        public async Task<ActionResult<practiceDTO>> PostLesson(practiceDTO lsn/*DateTime date, int StudentID, int TeacherID, int TypeID*/)
        {

            //
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (lsn.student_id == null) return BadRequest(ModelState);


                //если переданная меньше текущей на сервере - нельзя сделать запись в прошлое.
                if (lsn.date < DateTime.Now) { return StatusCode(406); }

                //if (await studentService.CheckCorrectTeacherAndCathegoryAsync(lsn.teacher_id, lsn.cathegory_id) == false)
                //{
                //    return StatusCode(422);
                //}
                if (studentService.IsBusyDate(lsn)) return StatusCode(409); //если дата у преподавателя занята
                practiceDTO les = new practiceDTO
                {
                    date = lsn.date,
                    stringDate = lsn.date.ToShortDateString() + " " + lsn.date.ToShortTimeString(),
                    /*
                     * type_id - статус занятия
                     * case 0: "Назначено"; break;
                     * case 1: "Отменено"; break;
                     * case 2: "Проведено"; break;
                     * default: "Неопредено"; break;
                     */
                    //type_id = 0,// при создании новго занятия - оно всегда должно иметь тип "Назначено"
                    //type = "Назначено",
                    student_id = lsn.student_id,
                    teacher_id = lsn.teacher_id,
                    //cathegory_id = lsn.cathegory_id,
                };
                les.id = await studentService.AddLessonAsync(les);
                les.teacherName = await studentService.GetTeacherNameByIdAsync(lsn.teacher_id);
                les.teacherPhoneNumber = await studentService.GetTeacherNumberByIdAsync(lsn.teacher_id);

                return CreatedAtAction("GetLesson", new { id = les.id }, les);
            }
            catch (Exception ex)
            {
                //логирование ошибки
                _logger.LogError(ex, "Ошибка при создании занятия");

                return StatusCode(500);
            }
        }

        // PUT api/<LessonsController>/5
        [HttpPut("{lessonId}/{typeId}")]
        public async Task<ActionResult> CancelAndUpdateLesson(int lessonId, int typeId)//id урока и type на который хотим сменить
        {
            try
            {
                var lesson = await studentService.GetLesson(lessonId);
                if (lesson == null) return StatusCode(404);//если нет такого занятия
                else if (lesson.lessonStatus == LessonStatus.Assigned)//0 = назначено. Если урок уже либо отменен, либо проведен, либо пр - возврат ошибки
                {
                    return StatusCode(400);
                }
                else
                {
                    await studentService.UpdateLessonAsync(lessonId, typeId);
                    return StatusCode(200);
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                _logger.LogError(ex, "Ошибка при обновлении занятия. Метод CancelAndUpdateLesson(int lessonId, int typeId)");
                return StatusCode(500);
            }
        }

        // DELETE api/<LessonsController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "teacher")]
        public async Task<HttpStatusCode> Delete(int id)
        {
            try
            {
                await studentService.DeleteLessonAsync(id);
                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                _logger.LogError(ex, "Ошибка при удалении занятия");
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
