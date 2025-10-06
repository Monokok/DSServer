using BLL.Services;
using DAL;
using DomainModel;
using Interfaces.DTO;
using Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public AccountController(
            UserManager<User> userManager,
            //SignInManager<User> signInManager,
            DrivingSchoolContext context,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _userManager = userManager;
            //_signInManager = signInManager;
            db = context;
            _configuration = configuration;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;

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
                    // Получаем список доступных ролей из RoleManager
                    var allowedRoles = await _roleManager.Roles.Select(r => r.Name.ToLower()).ToListAsync();

                    // Проверяем, существует ли указанная роль
                    if (!allowedRoles.Contains(model.Role.ToLower()))
                    {
                        return BadRequest(new
                        {
                            message = $"Недопустимая роль. Доступные роли: {string.Join(", ", allowedRoles)}."
                        });
                    }

                    Random random = new Random();
                    User user = new()
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        First_name = model.FirstName,
                        Middle_name = model.MiddleName,
                        Last_name = model.LastName,
                        TwoFactorEnabled = false,
                        PhoneNumber = model.PhoneNumber ?? $"7(910)000-50-{random.Next(10, 99)}"
                    };

                    // Добавление нового пользователя
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // Установка роли
                        await _userManager.AddToRoleAsync(user, model.Role.ToLower());

                        // Генерация токена
                        var token = await GenerateJwtToken(user);

                        return Ok(new { token, message = $"Добавлен новый пользователь: {user.UserName} с ролью {model.Role}" });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        var errorMessage = new
                        {
                            message = "Пользователь не добавлен",
                            error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                        };
                        return BadRequest(errorMessage);
                    }
                }
                else
                {
                    var errorMessage = new
                    {
                        message = "Неверные входные данные",
                        error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                    };
                    return BadRequest(errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в методе Register");
                return StatusCode(500);
            }
        }



        [HttpPost]
        [Route("api/account/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var token = await GenerateJwtToken(user);
                        var resultUser = new userDTO(user);

                        //Заполнение полей:

                        resultUser.user_roles = (await _userManager.GetRolesAsync(user)).ToList();
                        // Инициализируем переменную для основной роли
                        string primaryRole = "Не определена"; // Инициализация с начальным значением
                        // Проверяем, есть ли роль преподавателя
                        if (resultUser.user_roles.Contains("teacher"))
                        {
                            primaryRole = "Преподаватель";
                        }
                        // Если нет, проверяем роль студента
                        else if (resultUser.user_roles.Contains("student"))
                        {
                            primaryRole = "Обучающийся";
                        }
                        // Если нет, проверяем роль администратора
                        else if (resultUser.user_roles.Contains("admin"))
                        {
                            primaryRole = "Администратор";
                        }
                        // В resultUser можем добавить основную роль
                        resultUser.PrimaryRole = primaryRole;

                        resultUser.profileImage = GetProfileImageUrlByUserId(resultUser.id);



                        //Возврат
                        return Ok(new
                        {
                            token,
                            user = resultUser,
                        });
                    }
                    else
                    {
                        return Unauthorized("Invalid login attempt.");
                    }
                }
                else
                {
                    var errorMessage = new
                    {
                        message = "Вход не выполнен",
                        error = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                    };
                    return BadRequest(errorMessage);
                    //return Created("", errorMessage);
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

        [HttpPost]
        [Authorize]
        [Route("api/account/uploadProfileImage")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    // Извлекаем Id пользователя из токена (клейм NameIdentifier)
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    // Путь к папке, где будут храниться изображения
                    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    // Если папка не существует, создаем её
                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    // Получаем расширение файла
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    // Список разрешенных расширений
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

                    // Проверяем, что файл имеет разрешенное расширение
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = "Недопустимый формат файла" });
                    }

                    // Генерация имени файла
                    var fileName = $"{userIdClaim}{fileExtension}";
                    var filePath = Path.Combine(uploadsDirectory, fileName);

                    // Проверяем, есть ли уже файл изображения для этого пользователя
                    var existingImagePath = Path.Combine(uploadsDirectory, $"{userIdClaim}.*");
                    var existingImageFile = Directory.GetFiles(uploadsDirectory, $"{userIdClaim}.*").FirstOrDefault();

                    // Если файл существует, удаляем его
                    if (existingImageFile != null && System.IO.File.Exists(existingImageFile))
                    {
                        System.IO.File.Delete(existingImageFile);
                    }

                    // Сохранение нового файла
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Возвращаем путь к файлу
                    var fileUrl = $"/images/{fileName}?timestamp={DateTime.Now.Ticks}";

                    // Здесь можно обновить путь изображения в базе данных пользователя
                    var user = await _userManager.GetUserAsync(User);
                    user.ProfileImage = fileUrl;
                    await _userManager.UpdateAsync(user);

                    return Ok(new { profileImage = fileUrl });
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка загрузки файла: " + ex.Message);
            }

            return BadRequest("Ошибка загрузки файла.");
        }


        //[HttpPost]
        //[Authorize]
        //[Route("api/account/uploadProfileImage")]
        //public async Task<IActionResult> UploadProfileImage(IFormFile file)
        //{
        //    try
        //    {
        //        if (file != null && file.Length > 0)
        //        {
        //            // Извлекаем Id пользователя из токена (клейм NameIdentifier)
        //            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //            // Путь к папке, где будут храниться изображения
        //            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        //            // Если папка не существует, создаем её
        //            if (!Directory.Exists(uploadsDirectory))
        //            {
        //                Directory.CreateDirectory(uploadsDirectory);
        //            }

        //            // Получаем расширение файла
        //            var fileExtension = Path.GetExtension(file.FileName).ToLower();

        //            // Список разрешенных расширений
        //            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

        //            // Проверяем, что файл имеет разрешенное расширение
        //            if (!allowedExtensions.Contains(fileExtension))
        //            {
        //                return BadRequest(new { message = "Недопустимый формат файла" });
        //            }

        //            // Генерация имени файла
        //            var fileName = $"{userIdClaim}{fileExtension}";//Path.GetFileName(file.FileName);
        //            var filePath = Path.Combine(uploadsDirectory, userIdClaim);

        //            // Сохранение файла
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            // Возвращаем путь к файлу
        //            var fileUrl = $"/images/{userIdClaim}";

        //            // Здесь можно обновить путь изображения в базе данных пользователя
        //            var user = await _userManager.GetUserAsync(User);
        //            user.ProfileImage = fileUrl;
        //            await _userManager.UpdateAsync(user);

        //            return Ok(new { FileUrl = fileUrl });
        //        }
        //    } catch (Exception ex) {
        //        return BadRequest("Ошибка загрузки файла." + ex.Message);
        //    }
        //    return BadRequest("Ошибка загрузки файла.");


        //}

        // Получить аватарку в профиле пользователя
        [HttpGet("getProfileImage")]
        [Authorize]
        public IActionResult GetProfileImage()
        {
            // Получаем имя пользователя из контекста или передаем как параметр
            // Извлекаем Id пользователя из токена (клейм NameIdentifier)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userId = _userManager.; // Например, это может быть ID пользователя или его username
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", $"{userIdClaim}");

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound(new { message = "Изображение профиля не найдено" });
            }

            var fileBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(fileBytes, "image/jpeg");
        }

        [HttpGet("getUserPforileImage/{userId}")]
        public IActionResult GetProfileImageUrl(string userId)
        {
            try
            {
                string? fileUrl = GetProfileImageUrlByUserId(userId);
                if (fileUrl == null) return NotFound(new { message = "Image not found" });

                // Возвращаем URL изображения
                return Ok(new { profileImage = fileUrl });
            }
            catch (Exception ex)
            {
                return BadRequest("Error while retrieving image URL: " + ex.Message);
            }
        }

        public string? GetProfileImageUrlByUserId(string userId)
        {
            // Путь к папке, где хранятся изображения
            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // Ищем файл, который соответствует userId с любым расширением
            var filePath = Directory.GetFiles(uploadsDirectory, $"{userId}.*").FirstOrDefault();

            if (filePath == null)
            {
                return null;
            }

            // Извлекаем имя файла и формируем URL
            var fileName = Path.GetFileName(filePath);
            var profileImage = $"/images/{fileName}?timestamp={DateTime.Now.Ticks}";
            return profileImage ;
        }



        //Получение информации о профиле пользователя
        [HttpGet("getUserProfileById/{id}")]
        //[Authorize]
        public async Task<IActionResult> GetUserProfileById(string id)
        {
            // Извлекаем Id пользователя из токена (клейм NameIdentifier)
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //// Проверяем, совпадает ли идентификатор из токена с запрашиваемым
            //if (userIdClaim == null || userIdClaim != id)
            //{
            //    return Forbid(); // Возвращаем ошибку доступа, если пользователь пытается запросить не свои данные
            //}
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest();
                }

                var resultUser = new userDTO(user);

                //Заполнение полей:

                resultUser.user_roles = (await _userManager.GetRolesAsync(user)).ToList();
                // Инициализируем переменную для основной роли
                string primaryRole = "Не определена"; // Инициализация с начальным значением
                                                      // Проверяем, есть ли роль преподавателя
                if (resultUser.user_roles.Contains("teacher"))
                {
                    primaryRole = "Преподаватель";
                }
                // Если нет, проверяем роль студента
                else if (resultUser.user_roles.Contains("student"))
                {
                    primaryRole = "Обучающийся";
                }
                // Если нет, проверяем роль администратора
                else if (resultUser.user_roles.Contains("admin"))
                {
                    primaryRole = "Администратор";
                }
                // В resultUser можем добавить основную роль
                resultUser.PrimaryRole = primaryRole;

                resultUser.profileImage = GetProfileImageUrlByUserId(resultUser.id);

                return Ok(resultUser);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
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
