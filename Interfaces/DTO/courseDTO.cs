//using DomainModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Interfaces.DTO
//{
//    public class courseDTO
//    {
//        public courseDTO(Course course) 
//        { 
//            id = course.Id;
//            category_id = course.CathegoryId;
//            start_date = course.StartTime;
//            start_date_string = "Дата начала: " + course.StartTime.ToString("f");
//            end_time_string = "Дата завершения: " + course.EndTime.ToString("f");
//            end_time = course.EndTime;
//            cost = 0;// course.cost;
//            lecture_hours= course.LectureHours;
//            driving_hours = course.DrivingHours;
//            teacher_id = course.TeacherId;
//            student_count = 30;// course.student_count;//максимальное число зарегестрированных людей на курс - потенциальная функция на будущее
//            driving_hours_string = driving_hours + " часов практики";
//            lecture_hours_string = lecture_hours + " часов теории";
//            cost_string = "Цена:" + " " + cost.ToString() + " " + "₽";

//            registered_people = 0;//счётчик зарегестрированных на курс людей

//        }
//        public string category_teacher {  get; set; }
//        public int id { get; set; }
//        public int registered_people { get; set; }

//        public int category_id { get; set; }

//        public DateTime start_date { get; set; }

//        public DateTime end_time { get; set; }

//        public string start_date_string {  get; set; }
//        public string end_time_string {  get; set; }

//        public int cost { get; set; }
//        public string cost_string { get; set; }

//        public int lecture_hours { get; set; }

//        public int driving_hours { get; set; }

//        public string? teacher_id { get; set; }

//        public int student_count { get; set; }

//        public string teacher_name { get; set;}
//        public string category_name { get; set; }
//        public string category_fullname { get; set; }


//        public string driving_hours_string { get; set; }
//        public string lecture_hours_string { get; set; }
//    }
//}
