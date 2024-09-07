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


//�����������:
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration)
        .WriteTo.Console() // ������ ����� � �������
        .WriteTo.File("logs/DSServer.log", rollingInterval: RollingInterval.Day); // ������ ����� � ����
});

// Add services to the container.
//����� AddIdentity ������������� ����������� ������������, � ������� ����������� ���
//������������ � ��� ����. ��� ������������ ������������ User, ������� ��� ������ �����. �
//�������� ���� ���� ��������� ����������� IdentityRole.
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
    // ���������� 401 ��� ������ ����������� ������� ��� ����
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
builder.Services.AddSwaggerGen();//��������� SwaggerGen ������ � ��������� ����� ���������� ASP.NET Core. 



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
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseCors();//CORS
app.UseCors(builder => builder.AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
