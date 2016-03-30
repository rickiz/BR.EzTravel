using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BR.EzTravel.Web.CustomAttributes;

namespace BR.EzTravel.Web.Models
{
    #region Index

    public class PackageDetails
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public double Price { get; set; }
        public string ThumbnailImagePath { get; set; }
        public int ReviewCount { get; set; }
    }

    public class PackageSearchCriteria
    {
        public string FreeText { get; set; }

        [Display(Name = "Check In")]
        [DataType(DataType.Date)]
        [CustomDateDisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? CheckInDate { get; set; }

        [Display(Name = "Check Out")]
        [DataType(DataType.Date)]
        [CustomDateDisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? CheckOutDate { get; set; }
        public int CategoryID { get; set; }
        public double PriceFrom { get; set; }
        public double PriceTo { get; set; }
        public int[] Rates { get; set; }
        public int[] CityIDs { get; set; }
    }

    public class PackageCategory
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int Count { get; set; }
    }

    public class PackageIndexViewModel
    {
        public List<PackageDetails> SearchResults { get; set; }

        public List<PackageCategory> Categories { get; set; }

        public PackageSearchCriteria Criteria { get; set; }
    }

    #endregion

    #region Details

    public class PackageDetailsViewModel : PackageDetails
    {
        public string Category { get; set; }
        public DateTime StartDT { get; set; }
        public DateTime? EndDT { get; set; }
        public int Days { get; set; }
    }

    #endregion
}
