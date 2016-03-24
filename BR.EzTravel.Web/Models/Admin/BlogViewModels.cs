using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

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
    }

    public class BlogEditViewModel : BlogCreateViewModel
    {
        [Required]
        public int ID { get; set; }
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
    }
}
