using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Department.Models
{
    public class Progress
    {
        [Key]
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public Student Student { get; set; }
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        // public int? GroupId { get; set; }
        //public Group Group { get; set; }

        public string Mark { get; set; }

    }
}