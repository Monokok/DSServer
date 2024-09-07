using DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.DTO
{
    public class userDTO
    {
        public userDTO(User user)
        {
            isAuthenticated = true;
            value = user.First_name + " " + user.Middle_name + " " + user.Last_name + " " + user.Email;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            id = user.Id;
            name = user.First_name + " " + user.Last_name;
            first_name = user.First_name;
            middle_name = user.Middle_name;
            last_name = user.Last_name;
            a_hours = user.A_hours;
            b_hours = user.B_hours;
            c_hours = user.C_hours;
        }
        public bool isAuthenticated { get; set; }
        public string? id { get; set; }
        public int a_hours { get; set; }
        public string Email {  get; set; }
        public string PhoneNumber {  get; set; }
        public int b_hours { get; set; }
        public int c_hours { get; set; }
        public bool? TeachesCategoryA { get; set; }
        public bool? TeachesCategoryB { get; set; }
        public bool? TeachesCategoryC { get; set; }
        public string first_name { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        //public string tnumber { get; set; }
        public int paid_hours { get; set; }
        //public string? userRole { get; set; }

    }
}
