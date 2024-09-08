using BLL.Services;
using DAL;
using DomainModel;
using Interfaces.DTO;
using Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        //private readonly SignInManager<User> _signInManager;
        private readonly DrivingSchoolContext db;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<User> userManager,
            //SignInManager<User> signInManager,
            DrivingSchoolContext context,
            ILogger<AccountController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            //_signInManager = signInManager;
            db = context;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("role")]
        public async Task<IActionResult> GetUserRole()
        {
            // Извлекаем Id пользователя из токена (клейм NameIdentifier)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User not authorized or invalid token");
            }

            // Ищем пользователя по Id
            var user = await _userManager.FindByIdAsync(userIdClaim);

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




        //[Authorize]
        //[HttpGet("role")]
        //public async Task<IActionResult> GetUserRole()
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    if (user == null)
        //    {
        //        return NotFound("User not found");
        //    }

        //    // Проверка ролей пользователя
        //    if (await _userManager.IsInRoleAsync(user, "admin"))
        //    {
        //        return Ok("admin");
        //    }
        //    else if (await _userManager.IsInRoleAsync(user, "teacher"))
        //    {
        //        return Ok("teacher");
        //    }
        //    else if (await _userManager.IsInRoleAsync(user, "student"))
        //    {
        //        return Ok("student");
        //    }

        //    // Если роль не найдена
        //    return Unauthorized("User has no recognized role");
        //}


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
                        // Генерация токена
                        var token = await GenerateJwtToken(user);

                        //// Установка куки заменено на токен 07/09/2024
                        //await _signInManager.SignInAsync(user, false);
                        return Ok(new { token, message = "Добавлен новый пользователь: " + user.UserName });
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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                //    //обработка входа по данным
                //    var result = await _signInManager.PasswordSignInAsync(
                //        model.Email,
                //        model.Password,
                //        model.RememberMe,
                //        false
                //        );
                //    if (result.Succeeded) //если вход успешен
                //    {
                //        //string? id = db.Users.Where(i => i.Email == model.Email).FirstOrDefault().Id;
                //        var userByEmail = await _userManager.FindByEmailAsync(model.Email); //для отправки юзеру его данных
                //        if (userByEmail != null)
                //        {
                //            var token = await GenerateJwtToken(userByEmail);
                //            return Ok(new
                //            {
                //                token,
                //                user = new userDTO(userByEmail)
                //            });
                //        }
                //        else
                //        {
                //            return StatusCode(500);//если данные в бд невалидны (проблемы н-р с почтой)
                //        }
                //        //IList<string>? roles = await _userManager.GetRolesAsync(userByEmail);
                //        //string? userRole = roles.FirstOrDefault();
                //        //userDTO user = new userDTO(userByEmail);
                //        //user.userRole = userRole;
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                //        var errorMsg = new
                //        {
                //            message = "Вход не выполнен",
                //            error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                //        };
                //        return Created("", errorMsg);
                //    }
                //}
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var token = await GenerateJwtToken(user);
                        return Ok(new
                        {
                            token,
                            user = new userDTO(user)
                        });
                    }
                    else
                    {
                        return Unauthorized("Invalid login attempt.");
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
            // Для JWT аутентификации выход не требуется, так как клиент просто должен удалить токен
            return Ok(new { message = "Выполнен выход" });
            //try
            //{
            //    User usr = await GetCurrentUserAsync();
            //    if (usr == null)
            //    {
            //        return Unauthorized(new { message = "Сначала выполните вход" });
            //    }
            //    // Удаление куки
            //    await _signInManager.SignOutAsync();
            //    return Ok(new { message = "Выполнен выход", userName = usr.UserName });
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Ошибка в методе LogOff");
            //    return StatusCode(500);//, "Внутренняя ошибка сервера"
            //}
        }
        
        [Authorize]
        [HttpGet("api/account/isauthenticated")]
        public IActionResult IsAuthenticatedWithToken()
        {
            // Проверяем, аутентифицирован ли пользователь
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                // Извлекаем имя пользователя из токена (или другую информацию)
                var userName = HttpContext.User.Identity.Name;

                // Можно также извлекать другие клеймы из токена, если необходимо
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                return Ok(new
                {
                    isAuthenticated = true,
                    userName = userName,
                    userId = userIdClaim
                });
            }
            else
            {
                // Если пользователь не аутентифицирован
                return Unauthorized(new
                {
                    isAuthenticated = false,
                    message = "User is not authenticated"
                });
            }
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        // Метод для генерации JWT токена
        private async Task<string> GenerateJwtToken(User user)
        {
            // Объединяем имя, отчество и фамилию пользователя
            var fullName = $"{user.First_name} {user.Middle_name} {user.Last_name}";
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Используем user.Id как основной идентификатор
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, fullName),  // Добавляем имя пользователя в клеймы
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            }.Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
