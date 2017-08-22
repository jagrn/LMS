using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DocumentType DokumentType { get; set; }
        public string Format { get; set; }
        public DateTime UploadDate { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }
        public string UserId { get; set; }
    }


    public enum DocumentType
    {
        Beskrivning = 1,
        Föreläsning = 2,
        Instruktion = 3,
        Övningsuppgift = 4,
        Inlämningsuppgift = 5,
        Annat = 99
    }


}





