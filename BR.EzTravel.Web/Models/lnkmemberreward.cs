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
    
    public partial class lnkmemberreward
    {
        public int ID { get; set; }
        public int TransID { get; set; }
        public int MemberID { get; set; }
        public string TransType { get; set; }
        public int RewardPoint { get; set; }
        public System.DateTime CreateDT { get; set; }
    }
}
