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

        public static string GetFakeStudentId()
        {
            var students = db.Users.Where(u => u.Email == "student@lms.se").ToList();
            return students.First().Id;
        }

        private static void FakeCourseAttendance(int courseId)
        {
            //IdentityUserRole role = new IdentityUserRole();
            //role.RoleId = "Student";
            //var students = db.Users.Where(u => u.Roles.Contains(role)).ToList();

            var students = db.Users.Where(u => u.Email == "student@lms.se").ToList();
            students.First().CourseId = courseId;

            db.Entry(students.First()).State = EntityState.Modified;
            db.SaveChanges();        
        }

        // ADD a student notification to all students attending the concerned course
        public static void AddNoteToStudents(int courseId, int notificationId)
        {
            if ((courseId == 0) || (notificationId == 0))
                return;

            FakeCourseAttendance(courseId); ////////////////////////////

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
                var notification = db.Notifications.Where(n => n.Id == note.MyNoteRef).ToList();
                if (notification.First().EndOfRelevance >= DateTime.Now)
                {
                    notifications.Add(notification.First());
                }
            }
            return notifications;
        }

        // UPDATE a student notification for a student as read
        public static void UppdateNoteForStudent(string studentId, int notificationId)
        {
            if ((studentId == null) || (notificationId == 0))
                return;

            var studentNote = db.StudentNotifications.Where(sn => sn.ApplicationUserId == studentId)
                                    .Where(sn => sn.MyNoteRef == notificationId).ToList();

            studentNote.First().NoteRead = true;
        }

    }
}