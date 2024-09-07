using DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.DTO
{
    public class practiceDTO
    {
        public practiceDTO(Practice practice)
        {
            id = practice.Id;
            date = practice.Date;
            status = GetDescription(practice.Status);
            stringDate = date.ToShortDateString() + " " + date.ToShortTimeString();
            //Члены занятия
            teacher_id = practice.TeacherId;
            student_id = practice.StudentId;
            //Подробности
            studentPhoneNumber = "Номер обучающегося";
            teacherPhoneNumber = "Номер преподавателя";
            teacherName = "ФИО преподавателя";
            studentName = "ФИО студента";
            teacherEmail = "Email преподавателя";
            studentEmail = "Email обучающегося";
            lessonStatus = practice.Status;
        }
        public practiceDTO() { status = "Неопределено"; }
        public LessonStatus lessonStatus { get; set; }
        public string? studentPhoneNumber { get; set; }
        public string? teacherPhoneNumber { get; set; }
        public string? teacherEmail { get; set; }
        public string? studentEmail { get; set; }
        public string? teacherName { get; set; }
        public string? studentName { get; set; }
        public string? stringDate { get; set; }
        public string status { get; set; }
        public int id { get; set; }
        public DateTime date { get; set; }
        public string? teacher_id { get; set; }
        public string? student_id { get; set; }


        private string GetDescription(LessonStatus status)
        {
            return status switch
            {
                LessonStatus.Assigned => "Назначено",
                LessonStatus.Canceled => "Отменено",
                LessonStatus.Completed => "Проведено",
                _ => "Неопределено",
            };
        }
    }
}
