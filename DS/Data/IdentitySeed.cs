using DomainModel;
using Microsoft.AspNetCore.Identity;

namespace DS.Data
{
    public static class IdentitySeed
    {
        public static async Task CreateUserRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            
            // Создание ролей
            if (await roleManager.RoleExistsAsync("teacher") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("teacher"));
            }
            if (await roleManager.RoleExistsAsync("student") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("student"));
            }
            if (await roleManager.RoleExistsAsync("admin") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            // Создание Преподавателя 1
            string teacherEmail = "teacher@mail.com";
            string teacherPassword = "Aa123456!";
            if (await userManager.FindByNameAsync(teacherEmail) == null)
            {
                User teacher = new User
                {
                    Email = teacherEmail,
                    UserName = teacherEmail,
                    First_name = "Денис",
                    Middle_name = "Иванович",
                    Last_name = "Петров", //AccountType = 1,
                    //TeachesCategoryA = true, TeachesCategoryB = true, TeachesCategoryC = false,
                    PhoneNumber = "7(910)726-50-67"
                };
                IdentityResult result = await userManager.CreateAsync(teacher, teacherPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(teacher, "teacher");
                }
            }
            // Создание Преподавателя 2
            string teacherEmail2 = "teacher2@mail.com";
            string teacherPassword2 = "Aa123456!";
            if (await userManager.FindByNameAsync(teacherEmail2) == null)
            {
                User teacher = new User
                {
                    Email = teacherEmail2,
                    UserName = teacherEmail2,
                    First_name = "Роберт",
                    Middle_name = "Ровидович",
                    Last_name = "Китов",
                    //AccountType = 1,
                    //На какие категории ведёт занятия:
                    //TeachesCategoryA = false,
                    //TeachesCategoryB = true,
                    //TeachesCategoryC = true,
                    PhoneNumber = "7(910)568-53-52"

                };
                IdentityResult result = await userManager.CreateAsync(teacher, teacherPassword2);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(teacher, "teacher");
                }
            }

            // Создание Пользователя
            string userEmail = "student@mail.com";
            string userPassword = "Aa123456!";
            if (await userManager.FindByNameAsync(userEmail) == null)
            {
                User user = new User
                {
                    Email = userEmail,
                    UserName = userEmail,
                    First_name = "Иван",
                    Middle_name = "Иванович",
                    Last_name = "Иванов", //AccountType = 0,
                    PhoneNumber = "7(910)400-55-70\r\n"
                };
                IdentityResult result = await userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "student");
                }
            }
        }
    }
}
