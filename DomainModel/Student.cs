//using System;
//using System.Collections.Generic;

//namespace DomainModel;

//public partial class Student
//{
//    public int Id { get; set; }

//    public string FirstName { get; set; } = null!;

//    public string MiddleName { get; set; } = null!;

//    public string LastName { get; set; } = null!;

//    public int AHours { get; set; }

//    public int BHours { get; set; }

//    public int CHours { get; set; }

//    public virtual ICollection<CourseInvite> CourseInvites { get; set; } = new List<CourseInvite>();

//    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

//    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
//}
