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
    public class PackageCreateViewModel
    {
        public string ErrorMessage { get; set; }
    
        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required]
        public double Price { get; set; }

        [DisplayName("Category")]
        public int CategoryID { get; set; }

        [DisplayName("Country")]
        public int CountryID { get; set; }

        [Required]
        public string Description { get; set; }

        public int Days { get; set; }

        public int Nights { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [CustomDateDisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime StartDT { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        [CustomDateDisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? EndDT { get; set; }

        public List<SelectListItem> Countries { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public List<SelectListItem> Activities { get; set; }

        [Display(Name = "Activities")]
        public int[] SelectedActivities { get; set; }

        [DisplayName("Active")]
        public bool Active { get; set; }

        public string DetailImageNames { get; set; }
    }

    public class PackageEditViewModel : PackageCreateViewModel
    {
        public int ID { get; set; }
        public string ThumbnailImagePath { get; set; }

        public List<PackageEditMockFile> MockFiles { get; set; }
    }

    public class PackageEditMockFile
    {
        public string name { get; set; }
        public long size { get; set; }
    }

    public class AdminPackageIndexViewModel
    {
        public List<PackageIndexItem> Packages { get; set; }
    }

    public class PackageIndexItem
    {
        public string Title { get; set; }
        public int ID { get; set; }
        public DateTime LastUpdateDT { get; set; }
        public bool Active { get; set; }
        public int MemberID { get; set; }
    }
}
