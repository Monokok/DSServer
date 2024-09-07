using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL;

public partial class DrivingSchoolContext : IdentityDbContext<User>//DbContext
{
    protected readonly IConfiguration Configuration;
    public DrivingSchoolContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    //public virtual DbSet<Cathegory> Cathegories { get; set; }

    //public virtual DbSet<Course> Courses { get; set; }

    //public virtual DbSet<CourseInvite> CourseInvites { get; set; }

    //public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Practice> PracticeLessons { get; set; }

    //public virtual DbSet<LessonsType> LessonsTypes { get; set; }

    //public virtual DbSet<Payment> Payments { get; set; }

    //public virtual DbSet<PaymentsType> PaymentsTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=DrivingSchool;Trusted_Connection=True;Encrypt=False;");
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {

        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("dsusers"); // Устанавливаем имя таблицы для сущности User

            entity.Property(e => e.Id)
                .HasColumnName("id"); // Имя столбца в базе данных
            //.ValueGeneratedOnAdd() // Идентификатор пользователя задается вручную, а не генерируется автоматически

            entity.Property(e => e.A_hours).HasColumnName("A_hours"); // Свойство a_hours связано с столбцом A_hours
            entity.Property(e => e.B_hours).HasColumnName("B_hours"); // Свойство b_hours связано с столбцом B_hours
            entity.Property(e => e.C_hours).HasColumnName("C_hours"); // Свойство c_hours связано с столбцом C_hours

            entity.Property(e => e.First_name)
                .HasColumnType("text") // Определяем тип данных для столбца как текст
                .HasColumnName("first_name"); // Имя столбца в базе данных

            entity.Property(e => e.Last_name)
                .HasColumnType("text") // Определяем тип данных для столбца как текст
                .HasColumnName("last_name"); // Имя столбца в базе данных

            entity.Property(e => e.Middle_name)
                .HasColumnType("text") // Определяем тип данных для столбца как текст
                .HasColumnName("middle_name"); // Имя столбца в базе данных

            // Настройка связи: один пользователь может участвовать во множестве уроков вождения как студент
            entity.HasMany(u => u.StudentPractices) // Навигационное свойство PracticeLessons в User
                .WithOne(p => p.Student) // Указываем, что на другой стороне есть навигационное свойство, ссылающегося на User
                .HasForeignKey(p => p.StudentId) // Используем StudentId как внешний ключ
                .OnDelete(DeleteBehavior.ClientSetNull) // При удалении пользователя StudentId устанавливается в NULL
                .HasConstraintName("FK_practice_lessons_student"); // Имя внешнего ключа в базе данных

            // Настройка связи: один пользователь может участвовать во множестве практик как учитель
            entity.HasMany(u => u.TeacherPractices) // Навигационное свойство PracticeLessons в User
                .WithOne(p => p.Teacher)
                .HasForeignKey(p => p.TeacherId) // Используем TeacherId как внешний ключ
                .OnDelete(DeleteBehavior.ClientSetNull) // При удалении пользователя TeacherId устанавливается в NULL
                .HasConstraintName("FK_practice_lessons_teacher");
        });

        modelBuilder.Entity<Practice>(entity =>
        {
            // Имя таблицы в бд
            entity.ToTable("practice_lessons");

            // Создаем индексы для повышения производительности поиска по StudentId и TeacherId
            entity.HasIndex(e => e.StudentId, "IX_practice_lessons_student_id");
            entity.HasIndex(e => e.TeacherId, "IX_practice_lessons_teacher_id");

            // Настройка свойства Id как колонки 'id' в таблице
            entity.Property(e => e.Id).HasColumnName("id");

            // Настройка свойства Date как колонки 'date' с типом данных datetime
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");

            // Настройка свойства StudentId как колонки 'student_id'
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            // Настройка свойства TeacherId как колонки 'teacher_id'
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            // Связь между Practice и User, где User играет роль учителя
            entity.HasOne(d => d.Teacher)  // Указываем, что Practice имеет одного учителя
                .WithMany(u => u.TeacherPractices)  // И обратная сторона: учитель может иметь много практик.Навигационное свойство в User, указывающее на все практики, "где он учитель"
                .HasForeignKey(d => d.TeacherId)  // Связь через внешний ключ TeacherId
                .OnDelete(DeleteBehavior.ClientSetNull)  // При удалении учителя, связь не будет удалена, значение TeacherId будет установлено в NULL
                .HasConstraintName("FK_practice_lessons_teacher");  // Имя внешнего ключа в базе данных

            // Связь между Practice и User, где User играет роль студента
            entity.HasOne(d => d.Student)  // Указываем, что Practice имеет одного студента
                .WithMany(u => u.StudentPractices)  // И обратная сторона: студент может иметь много практик, но это не указано в User (поэтому пустое)
                .HasForeignKey(d => d.StudentId)  // Связь через внешний ключ StudentId
                .OnDelete(DeleteBehavior.ClientSetNull)  // При удалении студента, связь не будет удалена, значение StudentId будет установлено в NULL
                .HasConstraintName("FK_practice_lessons_student");  // Имя внешнего ключа в базе данных
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}



//modelBuilder.Entity<Cathegory>(entity =>
//{
//    entity.ToTable("cathegories");

//    entity.Property(e => e.Id)
//        .ValueGeneratedNever()
//        .HasColumnName("id");
//    entity.Property(e => e.Name)
//        .HasColumnType("text")
//        .HasColumnName("name");
//});

//modelBuilder.Entity<Course>(entity =>
//{
//    entity.ToTable("courses");

//    entity.HasIndex(e => e.CathegoryId, "IX_courses_cathegory_id");

//    entity.HasIndex(e => e.TeacherId, "IX_courses_teacher_id");

//    entity.Property(e => e.Id).HasColumnName("id");
//    entity.Property(e => e.CathegoryId).HasColumnName("cathegory_id");
//    entity.Property(e => e.DrivingHours).HasColumnName("driving_hours");
//    entity.Property(e => e.EndTime)
//        .HasColumnType("datetime")
//        .HasColumnName("end_time");
//    entity.Property(e => e.LectureHours).HasColumnName("lecture_hours");
//    entity.Property(e => e.StartTime)
//        .HasColumnType("datetime")
//        .HasColumnName("start_time");
//    entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

//    entity.HasOne(d => d.Cathegory).WithMany(p => p.Courses)
//        .HasForeignKey(d => d.CathegoryId)
//        .OnDelete(DeleteBehavior.ClientSetNull)
//        .HasConstraintName("FK_courses_cathegories");

//    entity.HasOne(d => d.User).WithMany(p => p.Courses)
//        .HasForeignKey(d => d.TeacherId)
//        .OnDelete(DeleteBehavior.ClientSetNull)
//        .HasConstraintName("FK_courses_dsuser");
//});

//modelBuilder.Entity<CourseInvite>(entity =>
//{
//    entity.HasKey(e => e.Id).HasName("PK_group_invites");

//    entity.ToTable("course_invites");

//    entity.HasIndex(e => e.CourseId, "IX_group_invites_group_id");

//    entity.HasIndex(e => e.StudentId, "IX_group_invites_student_id");

//    entity.Property(e => e.Id).HasColumnName("id");
//    entity.Property(e => e.CourseId).HasColumnName("course_id");
//    entity.Property(e => e.StudentId).HasColumnName("student_id");

//    entity.HasOne(d => d.Course).WithMany(p => p.CourseInvites)
//        .HasForeignKey(d => d.CourseId)
//        .OnDelete(DeleteBehavior.ClientSetNull)
//        .HasConstraintName("FK_course_invites_courses");

//    //entity.HasOne(d => d.CourseNavigation).WithMany(p => p.CourseInvites)
//    //    .HasForeignKey(d => d.CourseId)
//    //    .OnDelete(DeleteBehavior.ClientSetNull)
//    //    .HasConstraintName("FK_group_invites_groups");

//    entity.HasOne(d => d.User).WithMany(p => p.CourseInvites)
//        .HasForeignKey(d => d.StudentId)
//        .OnDelete(DeleteBehavior.ClientSetNull)
//        .HasConstraintName("FK_group_invites_dsuser");
//});

//modelBuilder.Entity<Group>(entity =>
//{
//    entity.ToTable("groups");

//    entity.HasIndex(e => e.CourseId, "IX_groups_course_id");

//    entity.Property(e => e.Id).HasColumnName("id");
//    entity.Property(e => e.CourseId).HasColumnName("course_id");

//    entity.HasOne(d => d.Course).WithMany(p => p.Groups)
//        .HasForeignKey(d => d.CourseId)
//        .OnDelete(DeleteBehavior.ClientSetNull)
//        .HasConstraintName("FK_groups_courses");
//});



//modelBuilder.Entity<LessonsType>(entity =>
//{
//    entity.ToTable("lessons_types");

//    entity.Property(e => e.Id)
//        .ValueGeneratedNever()
//        .HasColumnName("id");
//    entity.Property(e => e.Name)
//        .HasColumnType("text")
//        .HasColumnName("name");
//});

//modelBuilder.Entity<Payment>(entity =>
//{
//    entity.ToTable("payments");

//    entity.HasIndex(e => e.StudentId, "IX_payments_student_id");

//    entity.HasIndex(e => e.TypeId, "IX_payments_type_id");

//    entity.Property(e => e.Id).HasColumnName("id");
//    entity.Property(e => e.Date)
//        .HasColumnType("datetime")
//        .HasColumnName("date");
//    entity.Property(e => e.StudentId).HasColumnName("student_id");
//    entity.Property(e => e.LessonId).HasColumnName("lesson_id");
//    entity.Property(e => e.TypeId).HasColumnName("type_id");

//    entity.HasOne(d => d.User).WithMany(p => p.Payments) //FK с юзером (студентом)
//        .HasForeignKey(d => d.StudentId)
//        .OnDelete(DeleteBehavior.ClientSetNull)
//        .HasConstraintName("FK_payments_dsuser");

//    entity.HasOne(d => d.Type).WithMany(p => p.Payments) //FK с типом оплаты (оплата\возврат)
//        .HasForeignKey(d => d.TypeId)
//        .OnDelete(DeleteBehavior.ClientSetNull)
//        .HasConstraintName("FK_payments_payments_types");
//});

//modelBuilder.Entity<PaymentsType>(entity =>
//{
//    entity.ToTable("payments_types");

//    entity.Property(e => e.Id)
//        .ValueGeneratedNever()
//        .HasColumnName("id");
//    entity.Property(e => e.Name)
//        .HasColumnType("text")
//        .HasColumnName("name");
//});
