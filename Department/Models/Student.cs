using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Department.Models
{
    public class Student:User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int StudentCard { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public int? YearId { get; set; }
        public Year Year { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
        public Student()
        {
            Progresses = new List<Progress>();
        }
        public Student(int id, string name, int groupid, Group group, int yearid, Year year)
        {
            Id = id;
            Name = name;
            GroupId = groupid;
            Group = group;
            YearId = yearid;
            Year = year;
            Progresses = new List<Progress>();
        }
    }
}