using BLL.Services;
using DAL;
using DomainModel;
using Interfaces.DTO;
using Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//В конструкторе получаются сервисы UserManager и сервис SignInManager, которые
//аутентифицируют пользователя и устанавливать или удалять его cookie. Метод
//_userManager.CreateAsync добавляет в базу данных нового пользователя. Результат выполнения
//метода представляет класс IdentityResult. В случае если переданные параметры пользователя
//(электронная почта и пароли) не удовлетворяют требованиям, тогда он не будет добавлен. При
//удачном добавлении пользователя метод signInManager.SignInAsync() устанавливаем
//аутентификационные cookie для добавленного пользователя. При неудачном добавлении
//пользователя формируется ответ, содержащий все возникшие ошибки.

namespace DS.Controllers
{
    [Produces("application/json")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DrivingSchoolContext db;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, DrivingSchoolContext context, ILogger<AccountController> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            db = context;
        }

        [Authorize]
        [HttpGet("role")]
        public async Task<IActionResult> GetUserRole()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Проверка ролей пользователя
            if (await _userManager.IsInRoleAsync(user, "admin"))
            {
                return Ok("admin");
            }
            else if (await _userManager.IsInRoleAsync(user, "teacher"))
            {
                return Ok("teacher");
            }
            else if (await _userManager.IsInRoleAsync(user, "student"))
            {
                return Ok("student");
            }

            // Если роль не найдена
            return Unauthorized("User has no recognized role");
        }


        [HttpPost]
        [Route("api/account/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Random random = new Random();
                    User user = new()
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        A_hours = 0,
                        B_hours = 0,
                        C_hours = 0,
                        First_name = model.FirstName,
                        Middle_name = model.MiddleName,
                        Last_name = model.LastName,
                        TwoFactorEnabled = false,
                        //AccountType = 0, //тип аккаунта 1 = студент
                        PhoneNumber = "7(910)000-50-" + random.Next(10, 99),
                    };
                    // Добавление нового пользователя
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // Установка роли User
                        await _userManager.AddToRoleAsync(user, "student");
                        // Установка куки
                        await _signInManager.SignInAsync(user, false);
                        return Ok(new { message = "Добавлен новый пользователь: " + user.UserName });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        var errorMsg = new
                        {
                            message = "Пользователь не добавлен",
                            error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                        };
                        return Created("", errorMsg);
                    }
                }
                else
                {
                    var errorMsg = new
                    {
                        message = "Неверные входные данные",
                        error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                    };
                    return Created("", errorMsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в методе Register");
                return StatusCode(500);//, "Внутренняя ошибка сервера"
            }
        }
        [HttpPost]
        [Route("api/account/login")]
        //[AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        string? id = db.Users.Where(i => i.Email == model.Email).FirstOrDefault().Id;
                        var userByEmail = await _userManager.FindByEmailAsync(model.Email);
                        IList<string>? roles = await _userManager.GetRolesAsync(userByEmail);
                        string? userRole = roles.FirstOrDefault();
                        userDTO user = new userDTO(userByEmail);
                        //user.userRole = userRole;
                        return Ok(new { user });/*userName = model.Email, userRole, Id = id,*/

                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                        var errorMsg = new
                        {
                            message = "Вход не выполнен",
                            error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                        };
                        return Created("", errorMsg);
                    }
                }
                else
                {
                    var errorMsg = new
                    {
                        message = "Вход не выполнен",
                        error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                    };
                    return Created("", errorMsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в методе Login");
                return StatusCode(500);//, "Внутренняя ошибка сервера"
            }

        }
        [HttpPost]
        [Route("api/account/logoff")]
        public async Task<IActionResult> LogOff()
        {
            try
            {
                User usr = await GetCurrentUserAsync();
                if (usr == null)
                {
                    return Unauthorized(new { message = "Сначала выполните вход" });
                }
                // Удаление куки
                await _signInManager.SignOutAsync();
                return Ok(new { message = "Выполнен выход", userName = usr.UserName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в методе LogOff");
                return StatusCode(500);//, "Внутренняя ошибка сервера"
            }
        }
        [HttpGet]
        [Route("api/account/isauthenticated")]
        public async Task<IActionResult> IsAuthenticated()
        {
            try
            {

                User usr = await GetCurrentUserAsync();
                if (usr == null)
                {
                    return Unauthorized(new { message = "Вы Гость. Пожалуйста, выполните вход" });
                }
                IList<string> roles = await _userManager.GetRolesAsync(usr);
                string? userRole = roles.FirstOrDefault();
                userDTO user = new userDTO(usr);
                //user.userRole = userRole;
                return Ok(new { user });/*userName = usr.UserName, userRole, Id = usr.Id,*/
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в методе IsAuthenticated");
                return StatusCode(500);//, "Внутренняя ошибка сервера"
            }
        }
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
