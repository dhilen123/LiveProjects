//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LiveProjects.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TimeEntry
    {
        public int Id { get; set; }
        public System.DateTime PunchIn { get; set; }
        public Nullable<System.DateTime> PunchOut { get; set; }
        public Nullable<System.DateTime> BreakStartTime { get; set; }
        public Nullable<System.DateTime> BreakEndTime { get; set; }
        public string userId { get; set; }
        public Nullable<System.DateTime> Diffrnt { get; set; }
        public System.DateTime? breakDifff { get; set; }
        public TimeSpan? BreakTTime { get; set; }
        public TimeSpan? TotalTime { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
