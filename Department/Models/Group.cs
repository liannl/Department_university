using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Department.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public Group()
        {
            Students = new List<Student>();
        }
        public Group(int id, string name, ICollection<Student> students= null)
        {
            Id = id;
            Name = name;
            Students = students;
        }

    }
}