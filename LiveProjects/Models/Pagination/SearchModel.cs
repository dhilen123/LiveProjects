using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveProjects.Models.Pagination
{
    public class SearchModel
    {
        public string Startdate { get; set; }
        public string Enddate { get; set; }
        public int? resourceId { get; set; }
        public int? ProjectId { get; set; }
        public int? techGroupId { get; set; }
        public int? techid { get; set; }
    }
}