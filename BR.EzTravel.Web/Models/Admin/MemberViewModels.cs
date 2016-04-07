using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BR.EzTravel.Web.CustomAttributes;

namespace BR.EzTravel.Web.Models.Admin
{
    public class MemberCreateViewModel
    {
        [StringLength(20)]
        [Required]
        public string BusinessRegNo { get; set; }

        [StringLength(200)]
        [Required]
        public string CompanyName { get; set; }

        [StringLength(200)]
        [Required]
        public string PICName { get; set; }

        [StringLength(250)]
        [Required]
        public string PICEmail { get; set; }

        [StringLength(15)]
        [Required]
        public string PICContact { get; set; }

        [StringLength(500)]
        [Required]
        public string Address { get; set; }

        [StringLength(10)]
        [Required]
        public string Postcode { get; set; }

        [StringLength(50)]
        [Required]
        public string State { get; set; }

        [StringLength(50)]
        [Required]
        public string Country { get; set; }

        [StringLength(50)]
        [Required]
        public string Roles { get; set; }
        public DateTime CreatDT { get; set; }
        public DateTime ExpiryDT { get; set; }
        public bool Active { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class MemberEditViewModel : MemberCreateViewModel
    {
        [Required]
        public int ID { get; set; }
    }

    public class AdminMemberIndexViewModel
    {
        public List<AdminMemberIndexItem> Members { get; set; }
    }
    public class AdminMemberIndexItem
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
        public DateTime MemberSince { get; set; }
        public bool Active { get; set; }
    }
}