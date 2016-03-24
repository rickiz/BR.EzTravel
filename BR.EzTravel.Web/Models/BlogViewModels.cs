using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BR.EzTravel.Web.Models
{
    public class BlogIndexViewModel
    {
        public List<BlogDetails> Blogs { get; set; }
    }

    public class BlogDetails
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime LastEditedDate { get; set; }

        public string CreatedBy { get; set; }

        public int TotalComments { get; set; }
    }
}
