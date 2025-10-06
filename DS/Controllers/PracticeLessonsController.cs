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
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Globalization;


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
        //[Authorize(Roles = "student, teacher" )]
        [SwaggerOperation(
            Summary = "Получение списка занятий для пользователя по его id",
            Description = "Этот метод позволяет получить список занятий для пользователя по его id"
        )]

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
        //[Authorize(Roles = "user, teacher")]
        [SwaggerOperation(
            Summary = "Получение списка доступных вариантов записи к преподавателю на конкретный день",
            Description = "Этот метод позволяет получить список доступных вариантов времени для записи к преподавателю"
        )]
        public async Task<ActionResult<IEnumerable<string>>> GetTimesForLessonsAsync(string teacherId, string DayMonthYear)
        {
            try
            {
                DateTime selectedDate;

                // Попытаться преобразовать строку в DateTime с точным форматом
                if (DateTime.TryParseExact(DayMonthYear, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out selectedDate))
                {
                    // Получаем список всех доступных времён
                    var Times = await studentService.GetAvailableHours(teacherId, selectedDate);

                    // Текущее время
                    DateTime now = DateTime.Now;

                    // Если дата в прошлом, возвращаем пустой список
                    if (selectedDate < now.Date)
                    {
                        return new List<string>();
                    }

                    // Фильтруем времена, которые прошли, если это текущий день
                    List<string> times = Times
                        .Where(item => !(selectedDate.Date == now.Date && item < now))
                        .Select(item => item.ToShortTimeString())
                        .ToList();

                    //foreach (var item in Times)
                    //{
                    //    times.Add(item.ToShortTimeString());
                    //}
                    return times;
                }
                else
                {
                    return BadRequest("Некорректная дата.");
                }
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
        //[Authorize(Roles = "student, teacher")]
        [SwaggerOperation(
            Summary = "Создание занятия",
            Description = "Этот метод позволяет создать новое занятие, предоставив необходимые данные."
        )]
        public async Task<ActionResult<practiceDTO>> PostLesson([FromBody] LessonRequest request)
        {
            try
            {
                // Проверка модели
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.StudentId == null || request.TeacherId == null)
                {
                    return BadRequest("Student ID and Teacher ID are required.");
                }

                string[] formats = { "dd.MM.yyyy H:mm", "dd.MM.yyyy HH:mm" }; // Оба формата
                // Парсим строку в DateTime
                DateTime dateTime = DateTime.ParseExact(request.DateTime, formats, CultureInfo.InvariantCulture);



                // Получение информации о преподавателе
                var teachers = await studentService.GetTeachersList();
                var teacher = teachers.Where(t => t.id == request.TeacherId).FirstOrDefault();
                if (teacher == null)
                {
                    return BadRequest("Teacher is not exist");
                }
                var students = await studentService.GetStudentsListAsync();
                var student = students.Where(st => st.id == request.StudentId).FirstOrDefault();
                if (student == null)
                {
                    return BadRequest("Student is not exist");
                }

                // Проверка на запись в прошлое
                if (dateTime < DateTime.Now)
                {
                    return StatusCode(406); // Not Acceptable
                }


                // Проверка занятости преподавателя
                if (await studentService.IsBusyDateAsync(dateTime, request.StudentId, request.TeacherId))
                {
                    return StatusCode(409); // Conflict
                }

                // Создание нового занятия
                var newLesson = new practiceDTO
                {
                    date = dateTime,
                    stringDate = dateTime.ToString("g"), // Формат даты и времени
                    student_id = request.StudentId,
                    category = request.Category,
                    studentName = student.first_name + ' ' + student.middle_name + ' ' + student.last_name,
                    studentEmail = student.email,
                    studentPhoneNumber = student.number,
                    teacher_id = request.TeacherId,
                    teacherName = teacher.first_name + " " + teacher.middle_name + " " + teacher.last_name,
                    teacherEmail = teacher.email,
                    teacherPhoneNumber = teacher.number,
                    lessonStatus = LessonStatus.Assigned,
                    status = "Назначено",
                    description = "Практическое занятие по вождению",
                    title = "Запись на занятие",
                };

                // Добавление занятия в БД
                 var createdLessonId = await studentService.AddLessonAsync(newLesson);

                if (teacher == null)
                {
                    return NotFound("Teacher not found.");
                }

                // Формирование ответа
                var response = new practiceDTO
                {
                    id = createdLessonId,
                    date = newLesson.date,
                    stringDate = newLesson.date.ToString("g"), // Формат даты и времени
                    student_id = newLesson.student_id,
                    category = newLesson.category,
                    studentName = student.first_name + ' ' + student.middle_name + ' ' + student.last_name,
                    studentEmail = student.email,
                    studentPhoneNumber = student.number,
                    teacher_id = newLesson.teacher_id,
                    teacherName = teacher.first_name + " " + teacher.middle_name + " " + teacher.last_name,
                    teacherEmail = teacher.email,
                    teacherPhoneNumber = teacher.number,
                    lessonStatus = LessonStatus.Assigned,
                    status = "Назначено",
                    description = "Практическое занятие по вождению",
                    
                    title = "Запись на занятие",
                    
                    
                };

                return CreatedAtAction(nameof(PostLesson), new { id = response.id }, response);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                _logger.LogError(ex, "Ошибка при создании занятия");
                return StatusCode(500); // Internal Server Error
            }
        }

        // Модель запроса для нового занятия
        public class LessonRequest
        {
            public string DateTime { get; set; } = string.Empty;
            public string StudentId { get; set; } = string.Empty;
            public string TeacherId { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
        }

        // PUT api/<LessonsController>/5
        [HttpPut("{lessonId}")]
        [Authorize(Roles = "student, teacher")]
        [SwaggerOperation(
    Summary = "Отмена занятия",
    Description = "Этот метод позволяет отменить занятие. Обучающийся - не менее, чем за 24 часа, преподаватель - сразу."
)]
        public async Task<ActionResult> CancelTheLesson(int lessonId)//, int typeId)
        {
            // Получаем текущего пользователя из токена
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID пользователя из JWT токена
            var userRole = User.FindFirstValue(ClaimTypes.Role); // Получаем роль пользователя из JWT токена

            // Получаем занятие по ID
            var lessonsAsync = await studentService.GetAllMyLessons(userId);
            var lesson = 
                lessonsAsync.FirstOrDefault(l => l.id == lessonId);

            if (lesson == null)
            {
                return NotFound(new { Message = "Занятие не найдено." });
            }

            // Проверяем, может ли пользователь отменить занятие
            if (lesson.lessonStatus == LessonStatus.Canceled)
            {
                return BadRequest(new { Message = "Занятие уже отменено." });
            }

            // Получаем дату и время начала занятия
            var lessonStartTime = lesson.date;

            // Логика отмены занятия:
            if (userRole == "teacher")
            {
                // Преподаватель может отменить занятие в любой момент
                lesson.lessonStatus = LessonStatus.Canceled;
                //lesson.CancellationDate = DateTime.Now; // Устанавливаем дату отмены
            }
            else if (userRole == "student")
            {
                // Студент может отменить занятие только если до начала занятия остается более 24 часов
                if (lessonStartTime < DateTime.Now.AddHours(24))
                {
                    return BadRequest(new { Message = "Студент может отменить занятие не менее чем за 24 часа до его начала." });
                }
                lesson.lessonStatus = LessonStatus.Canceled;
                //lesson.CancellationDate = DateTime.Now; // Устанавливаем дату отмены
            }

            // Сохраняем изменения
            await studentService.UpdateLessonAsync(lesson.id, (int)LessonStatus.Canceled);

            // Возвращаем успешный ответ
            return Ok(new { Message = "Занятие успешно отменено." });
        }


        //// DELETE api/<LessonsController>/5
        //[HttpDelete("{id}")]
        //[Authorize(Roles = "teacher")]
        //public async Task<HttpStatusCode> Delete(int id)
        //{
        //    try
        //    {
        //        await studentService.DeleteLessonAsync(id);
        //        return HttpStatusCode.OK;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Логирование ошибки
        //        _logger.LogError(ex, "Ошибка при удалении занятия");
        //        return HttpStatusCode.InternalServerError;
        //    }
        //}
    }
}
