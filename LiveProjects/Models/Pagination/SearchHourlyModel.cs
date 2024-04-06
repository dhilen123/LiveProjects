using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveProjects.Models.Pagination
{
    public class SearchHourlyModel
    {
        public string Startdate { get; set; }
        public string Enddate { get; set; }
        public int? resourceId { get; set; }
        public int? ProjectId { get; set; }
        public int? techGroupId { get; set; }
        public int? roleId { get; set; }
        public int? underUtilize { get; set; }
        public int? overUtilize { get; set; }
    }
}