using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Department.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
        public Subject()
        {
            Progresses = new List<Progress>();
            Teachers = new List<Teacher>();
        }
        public Subject(int id, string name, ICollection<Progress> progresses = null, ICollection<Teacher> teacher = null)
        {
            Id = id;
            Name = name;
            Progresses = progresses;
            Teachers = teacher;
        }
    }
}