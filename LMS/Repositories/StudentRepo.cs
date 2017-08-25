using LMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LMS.Repositories
{
    public class StudentRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        //public static string GetFakeStudentId()
        //{
        //    var students = db.Users.Where(u => u.Email == "student@lms.se").ToList();
        //    return students.First().Id;
        //}

        public static void FakeCourseAttendance(int courseId, string studentId)
        {
            //IdentityUserRole role = new IdentityUserRole();
            //role.RoleId = "Student";
            //var students = db.Users.Where(u => u.Roles.Contains(role)).ToList();

            var students = db.Users.Where(u => u.Id == studentId).ToList();
            students.First().CourseId = courseId;

            db.Entry(students.First()).State = EntityState.Modified;
            db.SaveChanges();
        }

        public static void FakeStudentFullName(string studentId)
        {
            var students = db.Users.Where(u => u.Id == studentId).ToList();
            students.First().FirstName = students.First().Email;
            db.Entry(students.First()).State = EntityState.Modified;
            db.SaveChanges();
        }


        public static bool CheckRoleAs(string email, string valRole)
        {
            var user = db.Users.Where(u => u.Email == email).ToList();
            var roles = user.First().Roles.ToList();
            bool roleFound = false;

            foreach (var role in roles)
            {
                var roleId = role.RoleId;
                var roleName = db.Roles.Where(r => r.Id == roleId).ToList();
                if (roleName.First().Name == valRole)
                {
                    roleFound = true;
                }
            }

            if (roleFound)
                return true;
            else
                return false;
        }

        public static string GetStudentId(string email)
        {
            var students = db.Users.Where(u => u.Email == email).ToList();
            return students.First().Id;
        }

        public static ApplicationUser GetUser(string userId)
        {
            var users = db.Users.Where(u => u.Id == userId).ToList();
            return users.First();
        }

        public static int GetStudentCourse(string studentId)
        {
            var students = db.Users.Where(u => u.Id == studentId).ToList();
            return (int) students.First().CourseId;
        }

        public static string GetStudentName(string studentId)
        {
            var students = db.Users.AsNoTracking().Where(u => u.Id == studentId).ToList();
            return students.First().FullName;
        }

        public static bool IsCourseWithStudents(int courseId)
        {
            var students = db.Users.Where(u => u.CourseId == courseId).ToList();
            if (students.Count() == 0)
                return false;
            else
                return true;
        }

        // ADD a student notification to all students attending the concerned course
        public static void AddNoteToStudents(int courseId, int notificationId)
        {
            if ((courseId == 0) || (notificationId == 0))
                return;

            var students = db.Users.Where(u => u.CourseId == courseId).ToList();
            foreach (var student in students)
            {
                StudentNotification studentNote = new StudentNotification();
                studentNote.MyNoteRef = notificationId;
                studentNote.NoteRead = false;
                studentNote.ApplicationUserId = student.Id;
                db.StudentNotifications.Add(studentNote);
                db.SaveChanges();
            }       
        }

        // RETREIVE relevant and unread notifications for a student
        public static List<Notification> RetreiveNotesForStudent(string studentId)
        {
            if (studentId == null)
                return null;

            var studentNotes = db.StudentNotifications.Where(sn => sn.ApplicationUserId == studentId)
                                    .Where(sn => sn.NoteRead == false).ToList();

            List<Notification> notifications = new List<Notification>();
            foreach (var note in studentNotes)
            {
                var notification = NotificationRepo.RetrieveRelevantNote(note.MyNoteRef);
                if (notification != null)
                {
                    notifications.Add(notification);
                }
            }
            return notifications;
        }

        // UPDATE a student notification for a student as read
        public static void UpdateNoteForStudent(string studentId, int notificationId)
        {
            if ((studentId == null) || (notificationId == 0))
                return;

            var studentNote = db.StudentNotifications.Where(sn => sn.ApplicationUserId == studentId)
                                    .Where(sn => sn.MyNoteRef == notificationId).ToList();

            studentNote.First().NoteRead = true;

            db.Entry(studentNote.First()).State = EntityState.Modified;
            db.SaveChanges();
        }

        // UPDATE a all student notifications for a student as read
        public static void UpdateAllNotesForStudent(string studentId)
        {
            if (studentId == null)
                return;

            var studentNotes = db.StudentNotifications.Where(sn => sn.ApplicationUserId == studentId)
                                    .Where(sn => sn.NoteRead == false).ToList();

            foreach (var note in studentNotes)
            {
                note.NoteRead = true;
                db.Entry(note).State = EntityState.Modified;
            }
            db.SaveChanges();
        }

    }
}