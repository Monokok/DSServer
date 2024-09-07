using Microsoft.AspNetCore.Identity;
namespace DomainModel
{
    public class User : IdentityUser
    {
        public string First_name { get; set; } = null!;
        public string Middle_name { get; set; } = null!;
        public string Last_name { get; set; } = null!;
        public int A_hours { get; set; }
        public int B_hours { get; set; }
        public int C_hours { get; set; }

        // Навигационное свойство для практик, где пользователь является студентом
        public virtual ICollection<Practice> StudentPractices { get; set; } = new List<Practice>();

        // Навигационное свойство для практик, где пользователь является учителем
        public virtual ICollection<Practice> TeacherPractices { get; set; } = new List<Practice>();

    }
}

