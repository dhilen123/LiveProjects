using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveProjects.Models.Pagination
{
    public class ResourcePagination
    {
        public List<Resource> List { get; set; }
        public Pager Pager { get; set; }
    }
}