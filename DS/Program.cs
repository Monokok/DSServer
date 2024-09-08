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
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем JWT аутентификацию
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddCors(options => //CORS
{
    options.AddDefaultPolicy(
    builder =>
    {
        builder.WithOrigins("http://localhost:3000/")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        //IssuerSigningKey = new SymmetricSecurityKey(key)
    };
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


// Регистрация IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
//builder.Services.AddIdentity<User, IdentityRole>() //убрано 08/09/2024 - генерит куки identity
builder.Services.AddIdentityCore<User>(options =>
{
    // Настройки Identity
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<DrivingSchoolContext>()
.AddDefaultTokenProviders();

// Добавляем все необходимые службы
//builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();



// Убрано <07.09.2024> - переход на JWT-токены
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.Name = "DrivingSchoolApp";
//    options.LoginPath = "/";
//    options.AccessDeniedPath = "/";
//    options.LogoutPath = "/";
//    options.Events.OnRedirectToLogin = context =>
//    {
//        context.Response.StatusCode = 401;
//        return Task.CompletedTask;
//    };
//    // Возвращать 401 при вызове недоступных методов для роли
//    options.Events.OnRedirectToAccessDenied = context =>
//    {
//        context.Response.StatusCode = 401;
//        return Task.CompletedTask;
//    };
//});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});


//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Driving School API", Version = "v1" });

    // Добавляем схему для авторизации через JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Введите токен в формате: Bearer {токен}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
            //Array.Empty<string>()
        }
    });
});//добавляем SwaggerGen сервис в коллекцию служб приложения ASP.NET Core. 



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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Driving School API V1");
    });
}

//app.UseHttpsRedirection();

//app.UseCors();//CORS
app.UseCors(builder => builder.AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
