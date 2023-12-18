using Department.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Department.Models
{
    public class ProgressListViewModel
    {
        public IEnumerable<Progress> Progresses { get; set; }
        public SelectList Groups { get; set; }
        public SelectList Subjects { get; set; }
        public SelectList Years { get; set; }

    }
}