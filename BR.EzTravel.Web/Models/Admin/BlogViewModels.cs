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
    public class BlogCreateViewModel
    {
        [StringLength(200)]
        [Required]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Body { get; set; }

        [DisplayName("Category")]
        public int CategoryID { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }

    public class BlogEditViewModel : BlogCreateViewModel
    {
        [Required]
        public int ID { get; set; }
        public string ThumbnailImagePath { get; set; }
        [DisplayName("Active")]
        public bool Active { get; set; }
    }

    public class AdminBlogIndexViewModel
    {
        public List<AdminBlogIndexItem> Blogs { get; set; }
    }

    public class AdminBlogIndexItem
    {
        public string Title { get; set; }
        public int ID { get; set; }
        public DateTime LastUpdateDT { get; set; }
        public bool Active { get; set; }
        public int MemberID { get; set; }
    }
}
