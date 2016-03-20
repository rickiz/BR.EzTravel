﻿using System;
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
        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required]
        public double Price { get; set; }

        [DisplayName("Category")]
        public int CategoryID { get; set; }

        [DisplayName("Country")]
        public int CountryID { get; set; }

        public string Description { get; set; }

        public int Rate { get; set; }

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