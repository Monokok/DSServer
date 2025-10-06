using DomainModel;
using Interfaces.DTO;
using Interfaces.Repository;
using Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;

//using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class StudentService : IStudentService //сервис обучения
    {
        private IDbRepos db;
        private readonly UserManager<User> _userManager;
        public StudentService(IDbRepos repos, UserManager<User> userManager)
        {
            db = repos;
            _userManager = userManager;
        }

        public async Task<int> AddLessonAsync(practiceDTO lesson)
        {
            // Создаем новое занятие
            var newLesson = new Practice
            {
                Date = lesson.date,
                StudentId = lesson.student_id,
                TeacherId = lesson.teacher_id,
                Description = lesson.description,
                Title = lesson.title
            };

            try
            {
                // Сохраняем занятие в базе данных
                db.PracticeLessons.CreateAsync(newLesson);

                // Сохраняем изменения в базе и возвращаем ID нового занятия
                await db.Save();

                return newLesson.Id;
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Ошибка при добавлении занятия: {ex.Message}");
                throw;
            }
        }


        //public Task<bool> CheckCorrectTeacherAndCathegoryAsync(string teacher_id, int cathegory_id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<List<practiceDTO>> GetAllMyLessons(string user_id)//получение списка занятий для студента и преподавателя
        {
            // Получаем пользователя и его роли
            var user = await _userManager.FindByIdAsync(user_id);
            var userRoles = await _userManager.GetRolesAsync(user);

            // Проверяем, является ли пользователь студентом
            if (userRoles.Contains("student"))
            {
                // Получаем всех преподавателей и все занятия
                var teachers = await _userManager.GetUsersInRoleAsync("teacher");
                var lessons = await db.PracticeLessons.GetListAsync();

                // Фильтруем занятия для конкретного студента
                var studentLessons = lessons.Where(les => les.StudentId == user_id).ToList();

                // Преобразуем занятия в LessonDTO
                var lessonDTOs = studentLessons.Select(les => new practiceDTO(les)).ToList();

                // Заполняем данные преподавателей в LessonDTO
                foreach (var lesson in lessonDTOs)
                {
                    lesson.title = "Практическое занятие";
                    lesson.description = "Отработка навыков вождения";
                    lesson.studentPhoneNumber = user.PhoneNumber;
                    lesson.studentEmail = user.Email;
                    lesson.studentName = user.First_name + " " + user.Middle_name + " " + user.Last_name;
                    
                    var teacher = teachers.FirstOrDefault(t => t.Id == lesson.teacher_id);
                    if (teacher != null)
                    {
                        lesson.teacherName = $"{teacher.First_name} {teacher.Middle_name} {teacher.Last_name}";
                        lesson.teacherPhoneNumber = teacher.PhoneNumber;
                        lesson.teacherEmail = teacher.Email;
                    }
                }
                return lessonDTOs;
            }
            // Проверяем, является ли пользователь преподавателем
            else if (userRoles.Contains("teacher"))
            {
                // Получаем все занятия, относящиеся к этому преподавателю
                var teacherLessons = await db.PracticeLessons.GetListAsync();
                var teacherLessonDTOs = teacherLessons
                    .Where(lsn => lsn.TeacherId == user_id)
                    .Select(lsn => new practiceDTO(lsn))
                    .ToList();

                // Получаем всех студентов
                var students = await _userManager.GetUsersInRoleAsync("student");

                // Заполняем данные студентов в LessonDTO
                foreach (var lesson in teacherLessonDTOs)
                {
                    var student = students.FirstOrDefault(st => st.Id == lesson.student_id);
                    lesson.title = "Практическое занятие";
                    lesson.description = "Отработка навыков вождения";
                    lesson.teacherName = $"{user.First_name} {user.Middle_name} {user.Last_name}";
                    lesson.teacherPhoneNumber = user.PhoneNumber;
                    lesson.teacherEmail = user.Email;
                    if (student != null)
                    {
                        lesson.studentEmail = student.Email;
                        lesson.studentPhoneNumber = student.PhoneNumber;
                        lesson.studentName = $"{student.First_name} {student.Middle_name} {student.Last_name}";
                        lesson.studentPhoneNumber = student.PhoneNumber;
                    }
                }
                return teacherLessonDTOs;
            }

            // Если роль не определена, выбрасываем исключение
            throw new Exception("Неизвестный тип роли пользователя!");
        }
        public async Task<List<DateTime>> GetAvailableHours(string _teacher_id, DateTime _DayMonthYear)
        {
            // Получаем список уже занятых времён для указанного преподавателя
            var occupiedSlotsAsync = await db.PracticeLessons.GetListAsync();
            var occupiedSlots = occupiedSlotsAsync
                .Where(slot => slot.TeacherId == _teacher_id 
                    && 
                      slot.Date.Date == _DayMonthYear.Date 
                      &&
                      slot.Status == LessonStatus.Assigned
                      )
                .Select(slot => slot.Date.TimeOfDay)//только время
                .ToHashSet();//быстрый поиск

            // Задаем интервалы времени (например, каждые 1.5 часа)
            var startTime = new TimeSpan(8, 0, 0); // Начало записи (08:00)
            var endTime = new TimeSpan(18, 0, 0);  // Конец записи (18:00)
            var interval = TimeSpan.FromMinutes(90); // Интервал между занятиями 1.5 часа


            // Формируем список доступных слотов
            var availableSlots = new List<DateTime>();
            for (var time = startTime; time < endTime; time += interval)
            {
                if (!occupiedSlots.Contains(time)) // Проверяем, занято ли время
                {
                    availableSlots.Add(_DayMonthYear.Date + time); // Добавляем свободное время
                }
            }
            return availableSlots;
        }

        public Task<practiceDTO?> GetLesson(int lesson_id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<userDTO>> GetTeachersListAsync()
        {
            // Получение всех пользователей с ролью "Student"
            var students = await _userManager.GetUsersInRoleAsync("teacher");

            // Преобразование пользователей в DTO
            return students.Select(u => new userDTO(u)).ToList();
        }

        public async Task<List<userDTO>> GetStudentsListAsync()
        {
            // Получение всех пользователей с ролью "Student"
            var students = await _userManager.GetUsersInRoleAsync("student");

            // Преобразование пользователей в DTO
            return students.Select(u => new userDTO(u)).ToList();
        }
        public async Task<string> GetTeacherNameByIdAsync(string id)
        {
            //throw new NotImplementedException();
            // Поиск учителя по ID в базе данных
            var teacher = await db.Users.GetItemAsync(id);

            // Проверка, найден ли учитель
            if (teacher == null)
            {
                return null; // Или выбросьте исключение, если необходимо
            }

            // Возвращаем имя учителя
            return teacher.First_name + teacher.Middle_name + teacher.Last_name; // Предполагается, что у учителя есть свойство Name
        }

        public async Task<List<userDTO>> GetTeachersList()
        {
            var teachers = await _userManager.GetUsersInRoleAsync("teacher");
            return teachers.Select(teach => new userDTO(teach)).ToList();
        }



        public async Task<bool> IsBusyDateAsync(DateTime date, string studentId, string teacherId)
        {
            // Обрезаем секунды и миллисекунды для сравнения
            var dateToCompare = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);

            // Длительность занятия в минутах (например, 90 минут)
            var lessonDuration = TimeSpan.FromMinutes(90);

            // Получаем все занятия из базы
            var lessons = await db.PracticeLessons.GetListAsync();

            // Проверяем, пересекается ли указанное время с существующими занятиями
            var isTimeOverlapping = lessons.Any(l =>
                l.TeacherId == teacherId &&                 // Для того же преподавателя
                l.Status == LessonStatus.Assigned &&        // Только "назначенные" занятия
                l.Date.Date == dateToCompare.Date &&        // В тот же день
                (
                    // Указанная дата попадает в существующий интервал занятия
                    (dateToCompare >= l.Date && dateToCompare < l.Date.Add(lessonDuration)) ||

                    // Новый интервал пересекает существующее занятие
                    (l.Date >= dateToCompare && l.Date < dateToCompare.Add(lessonDuration))
                ));

            return isTimeOverlapping;
        }



        public async Task UpdateLessonAsync(int id, int type)
        {
            var lesson = await db.PracticeLessons.GetItemAsync(id);

            if (lesson != null)
            {
                lesson.Status = (LessonStatus)type;
                await db.Save();// ChangesAsync();
            }
            //throw new NotImplementedException();
        }

        
    }
};

