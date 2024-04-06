using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveProjects.Models.Pagination
{
    public class ProjectPagination
    {
        public List<Project> List { get; set; }
        public Pager Pager { get; set; }
    }
}