//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BR.EzTravel.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class lnkmemberpostcomment
    {
        public int ID { get; set; }
        public int MemberPostID { get; set; }
        public int MemberID { get; set; }
        public string Comments { get; set; }
        public System.DateTime CreateDT { get; set; }
        public Nullable<System.DateTime> UpdateDT { get; set; }
        public Nullable<System.DateTime> CancelDT { get; set; }
        public string Language { get; set; }
        public bool Active { get; set; }
    }
}
