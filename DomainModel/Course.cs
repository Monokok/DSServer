//using System;
//using System.Collections.Generic;

//namespace DomainModel;

//public partial class Course
//{
//    public int Id { get; set; }//?

//    public string? TeacherId { get; set; }

//    public int CathegoryId { get; set; }

//    public int LectureHours { get; set; }

//    public int DrivingHours { get; set; }

//    public DateTime StartTime { get; set; }

//    public DateTime EndTime { get; set; }

//    public virtual Cathegory Cathegory { get; set; } = null!;

//    public virtual ICollection<CourseInvite> CourseInvites { get; set; } = new List<CourseInvite>();

//    //public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

//    //public virtual Teacher Teacher { get; set; } = null!;
//    public virtual User User { get; set; } = null!;

//}
