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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            this.Allocations = new HashSet<Allocation>();
        }

        public int projId { get; set; }
        [DisplayName("Proejct Name")]
        [Required(ErrorMessage = "Please Enter Project Name ")]
        public string projName { get; set; }
        [DisplayName("Project Manager")]
        [Required(ErrorMessage = "Please Select Project Manager ")]
        public Nullable<int> projManager { get; set; }
        [DisplayName("Technology")]
        [Required(ErrorMessage = "Please Select Technology ")]
        public Nullable<int> technology { get; set; }
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Enter Start Date")]
        public Nullable<System.DateTime> startdate { get; set; }
        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Enter End Date ")]
        public Nullable<System.DateTime> enddate { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "Please Select Status ")]
        public string status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Allocation> Allocations { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual Technology Technology1 { get; set; }
    }
}