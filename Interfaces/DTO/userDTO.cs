using DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Interfaces.DTO
{
    public class userDTO
    {
        public userDTO(User user)
        {
            //isAuthenticated = true;
            //value = user.First_name + " " + user.Middle_name + " " + user.Last_name + " " + user.Email;
            email = user.Email;
            number = user.PhoneNumber;
            id = user.Id;
            first_name = user.First_name;
            middle_name = user.Middle_name;
            last_name = user.Last_name;
            name = $"{user.Last_name} {user.First_name} {user.Middle_name}";
            balance = user.Balance;
            registrationDate = user.RegistrationDate.ToString("dd.MM.yyyy");
            birthDate = user.BirthDate.ToString("dd.MM.yyyy");
            profileImage = user.ProfileImage;

    }
        public string name { get; set; }
        public string? profileImage { get; set; } // Путь к изображению
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public decimal balance { get; set; } // Баланс пользователя
        public string registrationDate { get; set; }// Дата регистрации
        public string birthDate { get; set; } // Дата рождения
        public List<string>? user_roles { get; set; }
        public string? id { get; set; }
        public string email {  get; set; }
        public string number {  get; set; }

        [JsonPropertyName("userRole")]
        public string PrimaryRole { get; set; } // Основная роль
    }
}
