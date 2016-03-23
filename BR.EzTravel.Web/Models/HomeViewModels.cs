using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BR.EzTravel.Web.Models
{
    public class HomeViewModel
    {
        public List<PackageDetails> LatestPackages { get; set; }

        public List<PackageDetails> PopularPackages { get; set; }
    }
}