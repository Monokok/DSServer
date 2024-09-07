using DS.Data;
using DAL;
using System.Text.Json.Serialization;
using Interfaces.Repository;
using Interfaces.Services;
using BLL.Services;
using DAL.Repository;
using Microsoft.AspNetCore.Identity;
using DomainModel;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => //CORS
{
    options.AddDefaultPolicy(
    builder =>
    {
        builder.WithOrigins("http://localhost:3000/")
     .AllowAnyHeader()
    .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<DrivingSchoolContext>();
builder.Services.AddScoped<IDbRepos, DbReposSQL>();

builder.Services.AddScoped<IStudentService, StudentService>(); //

builder.Services.AddControllers().AddJsonOptions(x =>
 x.JsonSerializerOptions.ReferenceHandler =
ReferenceHandler.IgnoreCycles);


//Логирование:
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration)
        .WriteTo.Console() // Запись логов в консоль
        .WriteTo.File("logs/DSServer.log", rollingInterval: RollingInterval.Day); // Запись логов в файл
});

// Add services to the container.
//Метод AddIdentity устанавливает стандартную конфигурацию, в которой указываются тип
//пользователя и тип роли. Тип пользователя используется User, который был создан ранее. В
//качестве типа роли выступает стандартный IdentityRole.
builder.Services.AddIdentity<User, IdentityRole>()
.AddEntityFrameworkStores<DrivingSchoolContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "DrivingSchoolApp";
    options.LoginPath = "/";
    options.AccessDeniedPath = "/";
    options.LogoutPath = "/";
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    // Возвращать 401 при вызове недоступных методов для роли
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});


//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();//добавляет SwaggerGen сервис в коллекцию служб приложения ASP.NET Core. 



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dsContext =
   scope.ServiceProvider.GetRequiredService<DrivingSchoolContext>();

   

    await DsContextSeed.SeedAsync(dsContext);
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeed.CreateUserRoles(userManager, roleManager); //Заполняем в сиде роли
                                                                  //scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //Swagger будет доступен только во время разработки
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseCors();//CORS
app.UseCors(builder => builder.AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
