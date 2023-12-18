using Department.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Department.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext db = new ApplicationContext();
        public ActionResult Index()
        {
           
            return View();
        }


#region progress
        [Authorize(Roles = "teacher")]
        public ActionResult Progress(int? group, int? subject, int? year)
        {
            string UserId = User.Identity.GetUserId();
            IQueryable<Progress> players = db.Progresses.Include(p => p.Student).Include(p1 => p1.Subject)
                .Include(p2 => p2.Teacher).Where(predicate: p3 => p3.Teacher.IndividualId == UserId);
            if (group != null && group != 0)
            {
                players = players.Where(p => p.Student.GroupId == group);
            }
            if (subject != null && subject != 0)
            {
                players = players.Where(p => p.SubjectId == subject);
            }
            if (year != null && year != 0)
            {
                players = players.Where(p => p.Student.YearId == year);
            }
            var sub = db.Subjects.Include(p1 => p1.Teachers).Where(p => p.Teachers.Any(l => l.IndividualId == UserId));

            List<Group> groups = db.Groups.ToList();
            List<Subject> subjects = sub.ToList();
            List<Year> years = db.Years.ToList();
           
            groups.Insert(0, new Group { Name = "Все", Id = 0 });
            subjects.Insert(0, new Subject { Name = "Все", Id = 0 });
            years.Insert(0, new Year { Name = "Все", Id = 0 });
            if (players == null)
            {
                return CreateProgress();
            }
                ProgressListViewModel plvm = new ProgressListViewModel
            {
                Progresses = players.ToList(),
                Groups = new SelectList(groups, "Id", "Name"),
                Subjects = new SelectList(subjects, "Id", "Name"),
                Years = new SelectList(years, "Id", "Name")
            };
            return View(plvm);
        }


        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult CreateProgress()
        {
            string UserId = User.Identity.GetUserId();
            SelectList students = new SelectList(db.Students, "Id", "Name");
            ViewBag.Students = students;
            var sub = db.Subjects.Include(p1 => p1.Teachers).Where(p => p.Teachers.Any(l => l.IndividualId ==UserId));

            SelectList subjects = new SelectList(sub, "Id", "Name");
            ViewBag.Subjects = subjects;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "teacher")]
        public ActionResult CreateProgress(Progress progress)
        {
            bool match = false;
            foreach (var item in db.Progresses)
            {
                if (progress.StudentId == item.StudentId && progress.SubjectId == item.SubjectId)
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                string UserId = User.Identity.GetUserId();
                var pogressUser = db.Teachers.FirstOrDefault(p3 => p3.IndividualId == UserId);

                progress.TeacherId = pogressUser.Id;
                db.Progresses.Add(progress);
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Progress");
                }

            }
            ViewBag.Message = "Оценка по этому предмету уже добавлена";
            return CreateProgress();

        }

        [HttpGet]
        [Authorize(Roles = "teacher")]
        public ActionResult EditProgress(int id = 0)
        {
            Progress progress = db.Progresses.Find(id);
            if (progress != null)
            {
                ViewBag.Student = db.Students.Find(progress.StudentId).Name;
                ViewBag.Subject = db.Subjects.Find(progress.SubjectId).Name;

                return View(progress);
            }
            return RedirectToAction("Progress");
        }

        [HttpPost]
        [Authorize(Roles = "teacher")]
        public ActionResult EditProgress(Progress progress)
        {
            Progress newProgress = db.Progresses.Find(progress.Id);
            newProgress.Mark = progress.Mark;

            db.Entry(newProgress).State = EntityState.Modified;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Progress");
            }
            return EditProgress(progress.Id);
        }
        [Authorize(Roles = "teacher")]
        public ActionResult DeleteProgress(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Progress progress = db.Progresses.Find(id);
            if (progress != null)
            {
                db.Progresses.Remove(progress);
                db.SaveChanges();
            }
            return RedirectToAction("Progress");
        }
        #endregion
        [Authorize(Roles = "admin")]
        public ActionResult Management()
        {
            return View(db.Progresses);
        }

#region group
        [Authorize(Roles = "admin")]
        public ActionResult Group()
        {
            return View(db.Groups);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreateGroup()
        {
            ViewBag.Students = db.Students.ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreateGroup(Group group, int[] selectedStudents)
        {
            bool match = false;
            foreach (var item in db.Groups)
            {
                if (group.Name == item.Name)
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                if (selectedStudents != null)
                {
                    foreach (var d in db.Students.Where(di => selectedStudents.Contains(di.Id)))
                    {
                        group.Students.Add(d);
                    }
                }
                db.Groups.Add(group);
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Group");
                }

            }
            ViewBag.Message = "Группа уже добавлена";
            return CreateGroup();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditGroup(int id = 0)
        {
            Group group = db.Groups.Find(id);
            if (group != null)
            {
                ViewBag.Students = db.Students.ToList();
                return View(group);
            }
            return RedirectToAction("Group");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditGroup(Group group, int[] selectedStudents)
        {
            Group newGroup = db.Groups.Find(group.Id);
            newGroup.Name = group.Name;
            bool match = false;
            foreach (var item in db.Groups)
            {
                if (group.Name == item.Name && group.Id != item.Id)
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                newGroup.Students.Clear();
                if (selectedStudents != null)
                {
                    foreach (var d in db.Students.Where(di => selectedStudents.Contains(di.Id)))
                    {
                        newGroup.Students.Add(d);
                    }
                }

                db.Entry(newGroup).State = EntityState.Modified;
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Group");
                }
            }
            return EditGroup(group.Id);
        }
        [Authorize(Roles = "admin")]
        public ActionResult DeleteGroup(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Group groups = db.Groups.Find(id);
            if (groups != null)
            {
                db.Groups.Remove(groups);
                db.SaveChanges();
            }
            return RedirectToAction("Group");
        }
#endregion

#region year
        [Authorize(Roles = "admin")]
        public ActionResult Year()
        {
            return View(db.Years);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreateYear()
        {
            ViewBag.Students = db.Students.ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreateYear(Year year, int[] selectedStudents)
        {
            bool match = false;
            foreach (var item in db.Years)
            {
                if (year.Name == item.Name)
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                if (selectedStudents != null)
                {
                    foreach (var d in db.Students.Where(di => selectedStudents.Contains(di.Id)))
                    {
                        year.Students.Add(d);
                    }
                }
                db.Years.Add(year);
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Year");
                }

            }
            ViewBag.Message = "Курс уже добавлен";
            return CreateYear();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditYear(int id = 0)
        {
            Year year = db.Years.Find(id);
            if (year != null)
            {
                ViewBag.Students = db.Students.ToList();
                return View(year);
            }
            return RedirectToAction("Year");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditYear(Year year, int[] selectedStudents)
        {
            Year newYear = db.Years.Find(year.Id);
            newYear.Name = year.Name;
            bool match = false;
            foreach (var item in db.Years)
            {
                if (year.Name == item.Name && year.Id != item.Id)
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                newYear.Students.Clear();
                if (selectedStudents != null)
                {
                    foreach (var d in db.Students.Where(di => selectedStudents.Contains(di.Id)))
                    {
                        newYear.Students.Add(d);
                    }
                }

                db.Entry(newYear).State = EntityState.Modified;
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Year");
                }
            }
            return EditYear(year.Id);
        }
        [Authorize(Roles = "admin")]
        public ActionResult DeleteYear(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Year year = db.Years.Find(id);
            if (year != null)
            {
                db.Years.Remove(year);
                db.SaveChanges();
            }
            return RedirectToAction("Year");
        }
        #endregion

#region teacher
        [Authorize(Roles = "admin")]
        public ActionResult Teacher()
        {
            return View(db.Teachers);
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreateTeacher()
        {
            ViewBag.Subjects = db.Subjects.ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreateTeacher(Teacher teacher, int[] selectedSubjects)
        {
            bool match = false;
            foreach (var item in db.Teachers)
            {
                if (teacher.Name == item.Name)
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                if (selectedSubjects != null)
                {
                    foreach (var d in db.Subjects.Where(di => selectedSubjects.Contains(di.Id)))
                    {
                        teacher.Subjects.Add(d);
                    }
                }
                db.Teachers.Add(teacher);
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Teacher");
                }
            }
            ViewBag.Message = "Преподаватель уже добавлен";
            return CreateTeacher();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditTeacher(int id = 0)
        {
            Teacher teacher = db.Teachers.Find(id);
            if (teacher != null)
            {
                ViewBag.Subjects = db.Subjects.ToList();
                return View(teacher);
            }
            return RedirectToAction("Teacher");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditTeacher(Teacher teacher, int[] selectedSubjects)
        {
            Teacher newTeacher = db.Teachers.Find(teacher.Id);
            newTeacher.Name = teacher.Name;

            newTeacher.Subjects.Clear();
            if (selectedSubjects != null)
            {
                foreach (var d in db.Subjects.Where(di => selectedSubjects.Contains(di.Id)))
                {
                    newTeacher.Subjects.Add(d);
                }
            }

            db.Entry(newTeacher).State = EntityState.Modified;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Teacher");
            }
            return EditYear(teacher.Id);
        }
        [Authorize(Roles = "admin")]
        public ActionResult DeleteTeacher(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Teacher teacher = db.Teachers.Find(id);
            if (teacher != null)
            {
                db.Teachers.Remove(teacher);
                db.SaveChanges();
            }
            return RedirectToAction("Teacher");
        }
        #endregion

#region subject
        [Authorize(Roles = "admin")]
        public ActionResult Subject()
        {
            return View(db.Subjects);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreateSubject()
        {
            ViewBag.Teachers = db.Teachers.ToList();
            ViewBag.Students = db.Students.ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreateSubject(Subject subject, int[] selectedTeachers)
        {
            bool match = false;
            foreach (var item in db.Subjects)
            {
                if (subject.Name == item.Name)
                {
                    match = true;
                    break;
                }
            }

            if (!match)
            {
                if (selectedTeachers != null)
                {
                    foreach (var d in db.Teachers.Where(di => selectedTeachers.Contains(di.Id)))
                    {
                        subject.Teachers.Add(d);
                    }
                }
                db.Subjects.Add(subject);
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Subject");
                }
            }
            ViewBag.Message = "Предмет уже добавлен";
            return CreateSubject();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditSubject(int id = 0)
        {
            Subject subject = db.Subjects.Find(id);
            if (subject != null)
            {
                ViewBag.Teachers = db.Teachers.ToList();
                return View(subject);
            }
            return RedirectToAction("Subject");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditSubject(Subject subject, int[] selectedTeachers)
        {
            Subject newSubject = db.Subjects.Find(subject.Id);
            newSubject.Name = subject.Name;
            bool match = false;

            foreach (var item in db.Subjects)
            {
                if (subject.Name == item.Name && subject.Id != item.Id)
                {
                    match = true;
                    break;
                }
            }

            if (!match)
            {
                newSubject.Teachers.Clear();
                if (selectedTeachers != null)
                {
                    foreach (var d in db.Teachers.Where(di => selectedTeachers.Contains(di.Id)))
                    {
                        newSubject.Teachers.Add(d);
                    }
                }

                db.Entry(newSubject).State = EntityState.Modified;
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Subject");
                }
            }
            return EditSubject(subject.Id);
        }
        [Authorize(Roles = "admin")]
        public ActionResult DeleteSubject(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Subject subject = db.Subjects.Find(id);
            if (subject != null)
            {
                db.Subjects.Remove(subject);
                db.SaveChanges();
            }
            return RedirectToAction("Subject");
        }
        #endregion
#region Student
        [Authorize(Roles = "admin")]
        public ActionResult Student()
        {
            return View(db.Students.Include(s1 => s1.Group).Include(s2 => s2.Year));
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreateStudent()
        {
            SelectList years = new SelectList(db.Years, "Id", "Name");
            ViewBag.Years = years;
            SelectList group = new SelectList(db.Groups, "Id", "Name");
            ViewBag.Groups = group;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreateStudent(Student student)
        {
            bool match = false;

            foreach (var item in db.Students)
            {
                if (student.StudentCard == item.StudentCard)
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                db.Students.Add(student);
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Student");
                }
            }
            return CreateProgress();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditStudent(int id = 0)
        {
            Student student = db.Students.Find(id);
            if (student != null)
            {
                SelectList years = new SelectList(db.Years, "Id", "Name");
                ViewBag.Years = years;
                SelectList group = new SelectList(db.Groups, "Id", "Name");
                ViewBag.Groups = group;

                return View(student);
            }
            //ViewBag.Message = "Студент уже добавлен";
            return RedirectToAction("Student");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditStudent(Student student)
        {
            Student newStudent = db.Students.Find(student.Id);

            db.Entry(newStudent).State = EntityState.Modified;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Student");
            }
            return EditProgress(student.Id);
        }
        [Authorize(Roles = "admin")]
        public ActionResult DeleteStudent(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Student student = db.Students.Find(id);
            if (student != null)
            {
                db.Students.Remove(student);
                db.SaveChanges();
            }
            return RedirectToAction("Students");
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}