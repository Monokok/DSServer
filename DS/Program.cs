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

// ��������� JWT ��������������
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


//�����������:
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration)
        .WriteTo.Console() // ������ ����� � �������
        .WriteTo.File("logs/DSServer.log", rollingInterval: RollingInterval.Day); // ������ ����� � ����
});


// ����������� IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
//builder.Services.AddIdentity<User, IdentityRole>() //������ 08/09/2024 - ������� ���� identity
builder.Services.AddIdentityCore<User>(options =>
{
    // ��������� Identity
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<DrivingSchoolContext>()
.AddDefaultTokenProviders();

// ��������� ��� ����������� ������
//builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();



// ������ <07.09.2024> - ������� �� JWT-������
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
//    // ���������� 401 ��� ������ ����������� ������� ��� ����
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

    // ��������� ����� ��� ����������� ����� JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "������� ����� � �������: Bearer {�����}",
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
});//��������� SwaggerGen ������ � ��������� ����� ���������� ASP.NET Core. 



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dsContext =
   scope.ServiceProvider.GetRequiredService<DrivingSchoolContext>();



    await DsContextSeed.SeedAsync(dsContext);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeed.CreateUserRoles(userManager, roleManager); //��������� � ���� ����
                                                                  //scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //Swagger ����� �������� ������ �� ����� ����������
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
