using DAL;
using DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DS.Controllers
{
    

    /// <summary>
    /// Контроллер, отвечающий за действия, производимые администратором
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
        private readonly DrivingSchoolContext db;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminsController(
             UserManager<User> userManager,
            //SignInManager<User> signInManager,
            DrivingSchoolContext context,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager
            )
        {
            _logger = logger;
            _userManager = userManager;
            //_signInManager = signInManager;
            db = context;
            _configuration = configuration;
            _roleManager = roleManager;
        }


        /// <summary>
        /// Делит одно число на другое.
        /// </summary>
        /// <param name="Course">Курс обучения</param>
        /// <returns>Результат деления.</returns>
        /// <exception cref="">.</exception>
        [HttpPost]
        [Route("api/account/register")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCourse()
        {
            return null;
        }
    }
}
