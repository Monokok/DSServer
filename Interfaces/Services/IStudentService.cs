using Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IStudentService
    {
        Task UpdateLessonAsync(int id, int type);//обновление занятия с Id = id с изменением его lessonType на type
        Task<bool> CheckCorrectTeacherAndCathegoryAsync(string teacher_id, int cathegory_id);//проверка Преподаёт ли teacher категорию с Id = cathegory_id
        List<DateTime> GetAvailableHours(string _teacher_id, DateTime _DayMonthYear); //получение доступных часов для записи к teacher в день _DayMonthYear
        bool IsBusyDate(practiceDTO lsn); //проверка занятости даты занятия - возможно на эту дату есть другое занятие, статус которого = "назначено"
        Task<List<practiceDTO>> GetAllMyLessons(string user_id); //получение всех занятий User'а с Id = user_id
        Task<practiceDTO?> GetLesson(int lesson_id);//получение подробностей о занятии
        Task<int> AddLessonAsync(practiceDTO lesson);//добавление занятия
        Task DeleteLessonAsync(int lesson_id);//удаление занятия
        Task<List<userDTO>> GetStudentsListAsync();

        Task<List<userDTO>> GetTeachersListAsync();

        Task<string> GetTeacherNameByIdAsync(string id);//получение имени преподавателя по его id
        Task<string> GetTeacherNumberByIdAsync(string id);//получение номера преподавателя по его id
        Task<List<userDTO>> GetTeachersList();//получение списка всех преподавателей
    }
}
