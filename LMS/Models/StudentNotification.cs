using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class StudentNotification
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int MyNoteRef { get; set; }
        public bool NoteRead { get; set; }      
    }
}