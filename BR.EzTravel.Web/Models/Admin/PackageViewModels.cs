using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BR.EzTravel.Web.Models.Admin
{
    public class PackageCreateViewModel
    {
        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required]
        public double Price { get; set; }

        public int CategoryID { get; set; }

        public int CountryID { get; set; }

        public string Description { get; set; }

        public List<SelectListItem> Countries { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }

    public class PackageEditViewModel : PackageCreateViewModel
    {
        public int ID { get; set; }
        public string ThumbnailImagePath { get; set; }
    }

    public class PackageIndexViewModel
    {
        public List<PackageIndexItem> Packages { get; set; }
    }

    public class PackageIndexItem
    {
        public string Title { get; set; }
        public int ID { get; set; }
        public DateTime LastUpdateDT { get; set; }
    }
}
