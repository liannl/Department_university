using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Department.Models
{
    public class ApplicationContext :IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("DepartmentDBConnection")
        { }
       
        public DbSet<RegistrationRole> RegistrationRoles { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Year> Years { get; set; }
        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }

    }
  
    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationContext>
    {
 
        protected override void Seed(ApplicationContext db)
        {
            RegistrationRole reg = new RegistrationRole("admin", "admin");
            RegistrationRole reg1 = new RegistrationRole("teacher", "teacher");
            db.RegistrationRoles.AddRange(new List<RegistrationRole> { reg, reg1 });

            Year year1 = new Year(1, "1");
            Year year2 = new Year(2, "2");
            Year year3 = new Year(3, "3");
            Year year4 = new Year(4, "4");
            db.Years.AddRange(new List<Year> { year1, year2, year3, year4 });

            Group group1 = new Group(1, "61");
            Group group2 = new Group(2, "62");
            Group group3 = new Group(3, "8");
            db.Groups.AddRange(new List<Group> { group1, group2, group3 });

            db.SaveChanges();

            Student student1 = new Student(1, "Рыкин Андрей Сергеевич", 1, group1, 3, year3);
            Student student2 = new Student(2, "Носов Хор Сергеевич", 3, group3, 1, year1);
            Student student3 = new Student(3, "Зотьева Марина Андреевна", 1, group1, 3, year3);
            db.Students.AddRange(new List<Student> { student1, student2, student3 });
            db.SaveChanges();



            Teacher teacher1 = new Teacher(1, "Абрамов Геннадий Владимирович");

            Subject subgect1 = new Subject
            {
                Id = 11,
                Name = "Компьютерные сети",
                Teachers = new List<Teacher>() { teacher1 }
            };



            db.Teachers.Add(teacher1);
            db.SaveChanges();
            db.Subjects.Add(subgect1);
            db.SaveChanges();

            Progress progress1 = new Progress
            {
                Id = 1,
                StudentId = 1,
                Student = student1,
                SubjectId = 1,
                Subject = subgect1,
                Teacher = teacher1,
                TeacherId = 1,
                Mark = "5"
            };
            Progress progress2 = new Progress
            {
                Id = 2,
                StudentId = 2,
                Student = student2,
                SubjectId = 1,
                Subject = subgect1,
                Teacher = teacher1,
                TeacherId = 1,
                Mark = "4"
            };
            db.Progresses.AddRange(new List<Progress> { progress1, progress2 });
            db.SaveChanges();


            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            // создаем две роли
            var role1 = new IdentityRole { Name = "admin" };
            var role2 = new IdentityRole { Name = "teacher" };
            var role3 = new IdentityRole { Name = "user" };
            // добавляем роли в бд
            roleManager.Create(role1);
            roleManager.Create(role2);
            roleManager.Create(role3);

            // создаем пользователей
            var admin = new ApplicationUser { Email = "admin@mail.ru", UserName = "Admin" };
            string password = "Admin_000";
            var result = userManager.Create(admin, password);

            // если создание пользователя прошло успешно
            if (result.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role1.Name);
            }
           
            base.Seed(db);
        }
    }
}