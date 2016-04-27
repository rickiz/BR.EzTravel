using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BR.EzTravel.Web.Models
{
    public class HomeViewModel
    {
        public List<PackageDetails> LatestPackages { get; set; }

        public List<PackageDetails> PopularPackages { get; set; }
        public List<BlogDetails> LatestBlogs { get; set; }
        public PackageSearchCriteria Criteria { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> PackageActivities { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}