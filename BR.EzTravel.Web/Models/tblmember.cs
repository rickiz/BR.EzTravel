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
    
    public partial class tblmember
    {
        public int ID { get; set; }
        public string BusinessRegNo { get; set; }
        public string CompanyName { get; set; }
        public string PICName { get; set; }
        public string PICContact { get; set; }
        public string PICEmail { get; set; }
        public string OfficeContact { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int AvailablePost { get; set; }
        public int RewardPoints { get; set; }
        public string Roles { get; set; }
        public System.DateTime CreateDT { get; set; }
        public System.DateTime ExpiryDT { get; set; }
    }
}