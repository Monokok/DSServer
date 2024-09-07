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

        public Task<int> AddLessonAsync(practiceDTO lesson)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckCorrectTeacherAndCathegoryAsync(string teacher_id, int cathegory_id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteLessonAsync(int lesson_id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<practiceDTO>> GetAllMyLessons(string user_id)//получение списка занятий для студента и преподавателя
        {
            //определить какая роль у юзера?
            throw new NotImplementedException();
            //if (user_id == "undefined") throw new Exception("Аккаунт Undefined!");
            //var account = await db.Users.GetItemAsync(user_id) ?? throw new Exception("Такого аккаунта в системе нет!");//получили аккаунт

            //// Получение пользователя по user_id
            //var user = await _userManager.FindByIdAsync(user_id);

            //switch (account.AccountType)//узнали кто обращается - тичер\юзер
            //{
            //    case 0: //студент
            //        var teachers = await GetTeachersList();
            //        var lessons = await db.Lessons.GetListAsync(); //получили все занятия
            //        lessons = lessons.Where(les => les.StudentId == user_id).ToList();//отфильтровали по конкретному студенту
            //        var DTOs = lessons.Select(les => new lessonDTO(les)).ToList();//сделали DTO
            //        foreach (var lsns in DTOs)
            //        {
            //            foreach (var tchrs in teachers)
            //            {
            //                if (lsns.teacher_id == tchrs.id)
            //                {
            //                    lsns.teacherName = tchrs.value;//кладём ФИО в DTO урока. name; value = ФИО. name = ИО
            //                    lsns.teacherPhoneNumber = tchrs.PhoneNumber;//кладём номер телефона в DTO урока
            //                }
            //            }
            //        }
            //        return DTOs;//вернули
            //    case 1: //преподаватель
            //        var allCurrentTeacherLessons = await db.Lessons.GetListAsync(); //нашли все занятия
            //        var allCurrentTeacherLessonsDTOs = allCurrentTeacherLessons.Where(lsn => lsn.TeacherId ==  user_id)
            //            .ToList()//оставили лишь те, которые принадлежат teacher'у
            //            .Select(i => new lessonDTO(i)).ToList();//сделали удобный DTO класс здорового человека.

            //        var students = await db.Users.GetListAsync();
            //        students = students.Where(st => st.AccountType == 0).ToList(); //нашли всех студентов
            //        //докидываем в уроки ФИО студентов
            //        foreach (var item in allCurrentTeacherLessonsDTOs)//перебираем все уроки
            //        {
            //            foreach (var st in students)//перебираем студентов
            //            {
            //                if (item.student_id == st.Id)
            //                {//подхватываем ФИО и номер телефона в DTO класс урока
            //                    item.studentName = st.FirstName + " " + st.MiddleName + " " + st.LastName + " " + st.Email;
            //                    item.studentPhoneNumber = st.PhoneNumber;
            //                }
            //            }
            //        }
            //        return allCurrentTeacherLessonsDTOs;
            //    default: throw new Exception("Неизвестный тип аккаунта!"); 
        }

        public List<DateTime> GetAvailableHours(string _teacher_id, DateTime _DayMonthYear)
        {
            throw new NotImplementedException();
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
        public Task<string> GetTeacherNameByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTeacherNumberByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<userDTO>> GetTeachersList()
        {
            throw new NotImplementedException();
        }

        public bool IsBusyDate(practiceDTO lsn)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLessonAsync(int id, int type)
        {
            throw new NotImplementedException();
        }
    }

        //public async Task<lessonDTO?> GetLesson(int lesson_id)
        //{
        //    var lesson = await db.Lessons.GetItemAsync(lesson_id);
        //    if (lesson == null) throw new Exception("Занятие с Id = {$lessons_id} не найдено!");
        //    else return new lessonDTO(lesson);
        //}
        //public async Task<int> AddLessonAsync(lessonDTO lesson)//при добавлении записи на занятие в таблице orders создаётся запись об "оплате"
        //{
        //    Lesson newLesson = new Lesson //создали занятие и сохранили в бд (уже с ID)
        //    {
        //        CathegoryId = lesson.cathegory_id,
        //        Date = lesson.date,
        //        StudentId = lesson.student_id,
        //        TeacherId = lesson.teacher_id,
        //        TypeId = lesson.type_id,
        //    };
        //     db.Lessons.CreateAsync(newLesson);
        //    await db.Save(); 
        //    var lessons = await db.Lessons.GetListAsync();

        //    //db.Payments.CreateAsync( //создали запись в таблице об оплате
        //    //    new Payment
        //    //    {
        //    //        Date = lesson.date,
        //    //          StudentId = lesson.student_id,
        //    //           TypeId = 0, //0 - оплачено
        //    //        LessonId = newLesson.Id,// ID берётся уже тот, что сделала для занятия сама БД
                     
        //    //    });
        //    //await db.Save();

        //    return lessons.Last().Id;
        //}

        //public async Task DeleteLessonAsync(int lesson_id)
        //{
        //    await db.Lessons.DeleteAsync(lesson_id);
        //    //удаляя запись на занятие удаление записи об оплате занятия не должно быть - факт записи и получения средств должен быть
        //}

        //public async Task<List<userDTO>> GetTeachersList()
        //{
        //    var teachers = await db.Users.GetListAsync();
        //    return teachers.Where(i => i.AccountType == 1).Select(i => new userDTO(i)).ToList();
        //}

        //public async Task<string> GetTeacherNameByIdAsync(string id)
        //{
        //    if (id == null) { throw new Exception("Переданный id преподавателя является null"); }
        //    var teacher = await db.Users.GetItemAsync(id);
        //    if (teacher != null) return teacher.LastName + " " + teacher.FirstName + " " + teacher.MiddleName + " " + teacher.Email;
        //    else throw new Exception("Не удалось найти преподавателя с таким id");
        //}

        
        //public List<DateTime> GetAvailableHours(string _teacher_id, DateTime _DayMonthYear)
        //{

        //    List<DateTime> hours = new List<DateTime>();
        //    for (int i = 0; i <= 20; i++) //время записи с 9 до 19:00
        //    {
        //        if (i == 0 && CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 9, 0, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 9, 0, 0));
        //        else if (i == 1
        //            && CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 9, 30, 0), _teacher_id)
        //            ) hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 9, 30, 0));
        //        else if (i == 2 && CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 10, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 10, 00, 0));
        //        else if (i == 3 && CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 10, 30, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 10, 30, 0));
        //        else if (i == 4 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 11, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 11, 00, 0));
        //        else if (i == 5 && CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 11, 30, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 11, 30, 0));
        //        else if (i == 6 && CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 12, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 12, 00, 0));
        //        else if (i == 7 && CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 12, 30, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 12, 30, 0));
        //        else if (i == 8 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 13, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 13, 00, 0));
        //        else if (i == 9 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 13, 30, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 13, 30, 0));
        //        else if (i == 10 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 14, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 14, 00, 0));
        //        else if (i == 11 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 14, 30, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 14, 30, 0));
        //        else if (i == 12 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 15, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 15, 00, 0));
        //        else if (i == 13 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 15, 30, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 15, 30, 0));
        //        else if (i == 14 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 16, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 16, 00, 0));
        //        else if (i == 15 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 16, 30, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 16, 30, 0));
        //        else if (i == 16 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 17, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 17, 00, 0));
        //        else if (i == 17 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 17, 30, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 17, 30, 0));
        //        else if (i == 18 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 18, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 18, 00, 0));
        //        else if (i == 19 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 18, 30, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 18, 30, 0));
        //        else if (i == 20 &&  CheckAvailableDateAsync(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 19, 00, 0), _teacher_id))
        //            hours.Add(new DateTime(_DayMonthYear.Year, _DayMonthYear.Month, _DayMonthYear.Day, 19, 00, 0));
        //        //TimeSpan timeSpan = TimeSpan.FromHours(i);
        //        //hours.Add());
        //    }
        //    return hours;
        //}

        //private bool CheckAvailableDateAsync(DateTime _time, string _teacher_id)
        //{
        //    var MLessonsOnTheSameDay = db.Lessons.GetList().Where(l => l.Date.Year == _time.Year && l.Date.Month == _time.Month && l.Date.Day == _time.Day && l.TeacherId == _teacher_id && l.TypeId == 0)//список занятий в тот же день
        //        .OrderBy(l => l.Date);//сортированнй по возрастанию

        //    List<lessonDTO> LessonsOnTheSameDay = new List<lessonDTO>();// = (List<lessonDTO>)MLessonsOnTheSameDay.Select(i => new lessonDTO(i));
        //    foreach (var item in MLessonsOnTheSameDay)
        //    {
        //        LessonsOnTheSameDay.Add(new lessonDTO(item));
        //    }

        //    lessonDTO ? LessonBefore = null, LessonAfter = null;
        //    if (LessonsOnTheSameDay.LastOrDefault(l => l.date <= _time) != null)
        //    {
        //        LessonBefore = LessonsOnTheSameDay.LastOrDefault(l => l.date <= _time);

        //    };//получаем последний урок, что меньше (по дате) текущей переданной даты для записи
        //    if (LessonBefore == null)//уроков до нету - сделаем урок с очень "старой" датой
        //        LessonBefore = new lessonDTO
        //        {
        //            date = new DateTime(1966, 1, 1),
        //            //car_id = 0,
        //            cathegory_id = 0,
        //             teacherName = "",
        //               student_id = "",
        //                teacher_id = "",
        //                 type_id = 0, 
                          
        //        };
        //    if (LessonsOnTheSameDay.FirstOrDefault(l => l.date >= _time) != null)
        //    {
        //        LessonAfter = LessonsOnTheSameDay.FirstOrDefault(l => l.date >= _time);
        //    }
        //    if (LessonAfter == null)//уроков после нету - сделаем урок с очень "будущей" датой
        //        LessonAfter = new lessonDTO
        //        {
        //            date = new DateTime(3995, 1, 1),
        //            cathegory_id = 0,
        //            teacherName = "",
        //            student_id = "",
        //            teacher_id = "",
        //            type_id = 0,
        //        };
        //    if ((_time - LessonBefore.date).TotalHours > 1 && (_time - LessonBefore.date).TotalMinutes >= 15) //смотрим разницу по времени с занятиемДо
        //    {
        //        if ((LessonAfter.date - _time).TotalHours > 1 && (LessonAfter.date - _time).TotalMinutes >= 15)//смотрим разницу по времени с занятиемПосле
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //public bool IsBusyDate(lessonDTO lsn)//true - если есть занятия "назначенные" (=занятый). false - если нет назначенных (=незанятый час|слот|время)
        //{
        //    //находим занятия которые будут у того же teacher
        //    //в тот же Date
        //    //и при этом эти занятии должны быть "назначены" а не отменены - если "отменены" - значит время доступно для записи.
        //    var lesson = db.Lessons.GetList().Where(l => l.Date == lsn.date && l.TeacherId == lsn.teacher_id && l.TypeId == 0).FirstOrDefault();
        //    if (lesson == null) return false;
        //    else return true;
        //}

        //public async Task<bool> CheckCorrectTeacherAndCathegoryAsync(string teacher_id, int cathegory_id)
        //{
        //    var teacher = await db.Users.GetItemAsync(teacher_id);
        //    if (teacher == null) return false;
        //    //cathegory_id = 0 = A, 1 = B, 2 = C
        //    switch (cathegory_id)
        //    {
        //        case 0:
        //            if (teacher.TeachesCategoryA == true) return true;//если запись на А и преподаватель рил обучает на А - true
        //            break;
        //        case 1:
        //            if (teacher.TeachesCategoryB == true) return true; break;
        //        case 2:
        //            if (teacher.TeachesCategoryC == true) return true; break;
        //        default:  break;
        //    }
        //    return false;
        //}

        //public async Task UpdateLessonAsync(int id, int type)//обновление занятия - отменено\проведено
        //{
        //    var lesson = await db.Lessons.GetItemAsync(id);
        //    if (lesson == null) throw new Exception("Переданный id занятия является null");
        //    lesson.TypeId = type;
        //    db.Lessons.Update(lesson);//обновив занятие - нужно при отмене в таблице оплат выставить "возврат средств"

        //    if (type != 1) //если мы не хотим отменить занятие (type != 1) - то изменения в оплате не трогаем и выходим
        //        return;
        //    ////Иначе обновление оплаты:
        //    //Payment? currentPayment;
        //    //var payment = await db.Payments.GetListAsync();//получили весь список
        //    //if (payment == null) throw new Exception("Не удалось найти какие-либо сведения об оплате занятий!");
        //    //currentPayment = payment.FirstOrDefault(i => i.LessonId == id);//находим оплату именно этого занятия
        //    //if (currentPayment == null) throw new Exception("Оплата занятия с ID = {$id} не найдена!");
        //    //else
        //    //{
        //    //    currentPayment.TypeId = 1;//1 = возврат средств за отмену
        //    //    db.Payments.Update(currentPayment);//обновляем данные
        //    //}
        //    //return;
        //}

        //public async Task<string> GetTeacherNumberByIdAsync(string id)
        //{
        //    if (id == null) { throw new Exception("Переданный id преподавателя является null"); }
        //    var teacher = await db.Users.GetItemAsync(id);
        //    if (teacher != null) return teacher.PhoneNumber;
        //    else throw new Exception("Не удалось найти преподавателя с таким id");
        //}
};

