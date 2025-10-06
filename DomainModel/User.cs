using Microsoft.AspNetCore.Identity;
namespace DomainModel
{
    public class User : IdentityUser
    {
        public string First_name { get; set; } = null!;
        public string Middle_name { get; set; } = null!;
        public string Last_name { get; set; } = null!;
        public decimal Balance { get; set; } // Баланс пользователя
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow; // Дата регистрации
        public DateTime BirthDate { get; set; } // Дата рождения
        public string ProfileImage { get; set; } = string.Empty; // Путь к изображению

        // Навигационное свойство для практик, где пользователь является студентом
        public virtual ICollection<Practice> StudentPractices { get; set; } = new List<Practice>();

        // Навигационное свойство для практик, где пользователь является учителем
        public virtual ICollection<Practice> TeacherPractices { get; set; } = new List<Practice>();

    }
}

