using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Department.Models
{
    public class Teacher : User
    {
        [Key]
        public int Id { get; set; }
        //public int? IndividualId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
        public Teacher()
        {
            Progresses = new List<Progress>();
            Subjects = new List<Subject>();
        }
        public Teacher(int id, string name, ICollection<Progress> progress= null, ICollection<Subject> subject= null)
        {
            Id = id;
            Name = name;
            Progresses = progress;
            Subjects = subject;
        }

    }
}